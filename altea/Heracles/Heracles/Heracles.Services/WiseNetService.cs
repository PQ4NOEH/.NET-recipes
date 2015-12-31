namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Altea.Classes.WiseNet;
    using Altea.Common.Classes;
    using Altea.Contracts;
    using Altea.Database;
    using Altea.Extensions;

#if WISENET_ALBACSS
    using Alba.CsCss;
#endif

    using Heracles.Net;

    public abstract partial class WiseNetService : Service<IWiseNetChannel>
    {
        private static readonly int WiseNetConnectionRetries;

        public static string GetDefaultSearchEngine(Guid userId, Language language)
        {
            string url;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISENET_GetDefaultSearchEngine]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@uid",
                    ParameterDirection.Input,
                    SqlDbType.UniqueIdentifier,
                    userId);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@language",
                    ParameterDirection.Input,
                    SqlDbType.Int,
                    language.GetDatabaseId());

                url = SqlDatabaseManager.ExecuteScalar<string>(command, SqlConnectionString.DataWarehouse);
            }

            return url;
        }
        
        public static HtmlParser GetDefaultParser()
        {
#if WISENET_HAP
            return HtmlParser.HtmlAgilityPack;
#elif WISENET_CSQUERY
            return HtmlParser.CsQuery;
#elif WISENET_ANGLESHARP
            return HtmlParser.AngleSharp;
#endif
        }

        #region Browser

        private static readonly string[] RequestHeaders =
        {
            "Accept-Language", "Cache-Control", "Pragma", "Last-Modified",
            "If-Match", "If-None-Match", "If-Unmodified-Since" 
        };

        private static readonly string[] ValidDateFormats =
        {
            "ddd MMM d HH:mm:ss yyyy GMT",  // RFC 822, updated by RFC 1123: Mon Jun 1 13:09:09 2009
            "dddd, dd-MMM-yy HH:mm:ss GMT", // RFC 850, obsoleted by RFC 1036: Monday, 01-Jun-09 13:09:09
            "ddd, dd MMM yyyy HH:mm:ss"     // ANSI C's asctime() format: Mon, 01 Jun 2009 13:09:09
        };

        private static readonly string InjectedHtmlAttribute;

        private const string InjectedHtmlAttributeValue = "true";

        private const string ForceWiseNetScript =
            @"if (window.location === window.parent.location) {{" +
            @"window.location.href='{0}';" +
            @"throw new Error('Not in WiseNet!');" +
            @"}}";

        static WiseNetService()
        {
            WiseNetService.WiseNetConnectionRetries =
                Convert.ToInt32(ConfigurationManager.AppSettings["WiseNetConnectionRetries"]);

            // Bug in Uri class. Workaround found in: http://stackoverflow.com/questions/856885/httpwebrequest-to-url-with-dot-at-the-end
            MethodInfo getSyntax = typeof(UriParser).GetMethod("GetSyntax", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo flagsField = typeof(UriParser).GetField("m_Flags", BindingFlags.Instance | BindingFlags.NonPublic);

            if (getSyntax != null && flagsField != null)
            {
                foreach (string scheme in new[] { "http", "https" })
                {
                    UriParser parser = (UriParser)getSyntax.Invoke(null, new object[] { scheme });
                    if (parser != null)
                    {
                        int flagsValue = (int)flagsField.GetValue(parser);

                        // Clear the CanonicalizeAsFilePath attribute
                        if ((flagsValue & 0x1000000) != 0)
                        {
                            flagsField.SetValue(parser, flagsValue & ~0x1000000);
                        }
                    }
                }
            }
            
            using (HashAlgorithm hash = MD5.Create())
            {
                InjectedHtmlAttribute =
                    "wdata-"
                    + hash
                        .ComputeHash(Encoding.Default.GetBytes("wisenet"))
                        .Aggregate(
                            new StringBuilder(),
                            (sb, b) => sb.Append(b.ToString("x2")));
            }
        }

        public static WiseNetDocument GetDocument(
            HtmlParser parser,
            Uri uri,
            HttpMethod method,
            HttpRequestBase baseRequest,
            bool isDeveloper)
        {
            if (uri == null)
            {
                throw new ArgumentException(@"A valid URI must be provided.", "uri");
            }

            if (UriSchemeConverter.Convert(uri.Scheme) == UriScheme.NotSupported)
            {
                throw new ArgumentException(@"Not suported URI scheme.", "uri");
            }

            if (method == HttpMethod.NotSupported || !Enum.IsDefined(typeof(HttpMethod), method))
            {
                throw new ArgumentException(@"Not supported HTTP method.", "method");
            }

            VirtualPathData virtualPath = RouteTable.Routes.GetVirtualPathForArea(
                baseRequest.RequestContext,
                "WiseNet_proxy",
                null);

            // ReSharper disable once PossibleNullReferenceException
            Uri actionUri = new Uri(virtualPath.VirtualPath, UriKind.Relative);
            // ReSharper disable once PossibleNullReferenceException
            Uri authorityUri = new Uri(baseRequest.Url.GetLeftPart(UriPartial.Authority), UriKind.Absolute);
            Uri browserUri = new Uri(authorityUri, actionUri);

            NameValueCollection getParams = HttpUtility.ParseQueryString(uri.Query);
            FixHtmlValueCollection(getParams);
            NameValueCollection parameters = new NameValueCollection(Math.Max(getParams.Count, baseRequest.Form.Count));
            FixHtmlValueCollection(parameters);

            switch (method)
            {
                case HttpMethod.Get:
                    parameters.Add(getParams);
                    parameters.Add(baseRequest.Form);
                    break;

                case HttpMethod.Post:
                    parameters.Add(baseRequest.Form);
                    break;
            }

            BlacklistData blacklist = LoadBlacklist(uri, method, parameters);
            HttpWebRequest request = CreateRequest(uri, method, parameters, blacklist, baseRequest, browserUri);
            HttpWebResponse response = null;

            int count = WiseNetService.WiseNetConnectionRetries;

            while (count > 0)
            {
                try
                {
                    response = request.GetResponse() as HttpWebResponse;
                    break;
                }
                catch (WebException ex)
                {
                    bool status;

                    switch (ex.Status)
                    {
                        case WebExceptionStatus.ProtocolError:
                            // Responses with status codes different from 200 make GetResponse()
                            // throw a WebException. Some websites return pages with info, so we
                            // also want to parse those pages instead of throwing an exception.
                            response = ex.Response as HttpWebResponse;
                            status = true;
                            break;

                        case WebExceptionStatus.ConnectFailure:
                        case WebExceptionStatus.ConnectionClosed:
                        case WebExceptionStatus.Pending:
                        case WebExceptionStatus.Timeout:
                        case WebExceptionStatus.SecureChannelFailure:
                        case WebExceptionStatus.SendFailure:
                        case WebExceptionStatus.ReceiveFailure:
                            status = false;
                            break;

                        default:
                            throw;
                    }

                    if (status)
                    {
                        break;
                    }
                }

                count--;
            }

            if (response == null)
            {
                throw new NullReferenceException("WiseNet: response is null after exiting from load loop.");
            }

            WiseNetDocument documentData = new WiseNetDocument
            {
                // ReSharper disable once PossibleNullReferenceException
                Url = response.ResponseUri,
                StatusCode = response.StatusCode,
                StatusDescription = response.StatusDescription,
                ContentType = response.ContentType,
                Document = null,
                DocumentStream = null
            };

            if (documentData.ContentType.StartsWith("text/html"))
            {
                HtmlParser selectedParser =
                    parser == HtmlParser.Default
                        ? GetParserForUri(response.ResponseUri)
                        : parser;

                documentData.Document =
                    (string)typeof(WiseNetService).GetMethod(
                        (selectedParser == HtmlParser.Default ? GetDefaultParser() : parser) + "_RenderHtmlDocument",
                        BindingFlags.NonPublic | BindingFlags.Static)
                        .Invoke(null, new object[] { response, blacklist, documentData.Url, baseRequest, selectedParser, isDeveloper });
            }
            else if (documentData.ContentType.StartsWith("text/css"))
            {
                using (Stream stream = response.GetResponseStream())
                // ReSharper disable once AssignNullToNotNullAttribute
                using (TextReader reader = new StreamReader(stream))
                {
                    documentData.Document = ParseCssUris(reader.ReadToEnd(), documentData.Url, baseRequest);
                }

            }
            else
            {
                using (Stream stream = response.GetResponseStream())
                {
                    if (stream == null)
                    {
                        documentData.DocumentStream = null;
                    }
                    else
                    {
                        documentData.DocumentStream = new MemoryStream();
                        stream.CopyTo(documentData.DocumentStream);
                        documentData.DocumentStream.Position = 0;
                    }
                }
            }

            response.Dispose();
            return documentData;
        }

        private static BlacklistData LoadBlacklist(Uri uri, HttpMethod method, NameValueCollection parameters)
        {
            BlacklistData blacklist = null;

            string authority = uri.GetLeftPart(UriPartial.Authority),
                   path      = uri.GetLeftPart(UriPartial.Path);

            using (SqlCommand command = SqlDatabaseManager.CreateCommand(CommandType.StoredProcedure, "[dbo].[WISENET_GetBlacklistData]"))
            using (DataTable queryParameters = new DataTable())
            {
                queryParameters.Columns.Add("k", typeof(string));
                queryParameters.Columns.Add("v", typeof(string));

                foreach (string key in parameters)
                {
                    DataRow row = queryParameters.NewRow();
                    row["k"] = key;
                    row["v"] = parameters[key];
                    queryParameters.Rows.Add(row);
                }

                SqlDatabaseManager.AddParameter(
                    command,
                    "@method",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    method.ToString().ToLowerInvariant());

                SqlDatabaseManager.AddParameter(
                    command,
                    "@authority",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    authority);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@path",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    path);

                SqlDatabaseManager.AddParameter(
                    command,
                    "@query",
                    ParameterDirection.Input,
                    "[dbo].[keyvaluelist]",
                    queryParameters);

                SqlDatabaseManager.ExecuteReader(
                    command,
                    SqlConnectionString.DataWarehouse,
                    reader =>
                        {
                            if (!reader.Read())
                            {
                                blacklist = new BlacklistData
                                    {
                                        Strength = BlacklistStrength.None,
                                        AddRequestParameters = new Dictionary<string, string>(),
                                        RemoveRequestParameters = Enumerable.Empty<string>(),
                                        RemoveTags = Enumerable.Empty<string>(),
                                        RemoveAttributes = Enumerable.Empty<string>(),
                                        InjectHeadCode = string.Empty
                                    };

                                return;
                            }

                            string addRequestParameters = reader["add_request_parameters"] as string,
                                   removeRequestParameters = reader["remove_request_parameters"] as string,
                                   removeTags = reader["remove_tags"] as string,
                                   removeAttributes = reader["remove_attributes"] as string;

                            blacklist = new BlacklistData
                                {
                                    Strength = (BlacklistStrength)(int)reader["strength"],
                                    AddRequestParameters =
                                        addRequestParameters == null
                                            ? new Dictionary<string, string>()
                                            : addRequestParameters.Split('&')
                                                    .Select(x => x.Split('='))
                                                    .ToDictionary(k => k[0], v => v[1]),
                                    RemoveRequestParameters =
                                        removeRequestParameters == null
                                            ? Enumerable.Empty<string>()
                                            : removeRequestParameters.Split('&'),
                                    RemoveTags =
                                        removeTags == null
                                            ? Enumerable.Empty<string>()
                                            : removeTags.Split('&'),
                                    RemoveAttributes =
                                        removeAttributes == null
                                            ? Enumerable.Empty<string>()
                                            : removeAttributes.Split('&'),
                                    InjectHeadCode = reader["inject_head_code"] as string,
                                };
                        });
            }

            return blacklist;
        }

        private static HttpWebRequest CreateRequest(
            Uri uri,
            HttpMethod method,
            NameValueCollection parameters,
            BlacklistData blacklist,
            HttpRequestBase baseRequest,
            Uri browserUri)
        {
            foreach (string parameter in blacklist.RemoveRequestParameters)
            {
                parameters.Remove(parameter);
            }

            string queryParams = string.Empty;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (string key in parameters)
            {
                queryParams += "&" + key +
                    (parameters[key] == null ? string.Empty : "=" + parameters[key]);
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (KeyValuePair<string, string> parameter in blacklist.AddRequestParameters)
            {
                queryParams += "&" + Uri.EscapeDataString(parameter.Key) +
                    (parameter.Value == null ? string.Empty : "=" + Uri.EscapeDataString(parameter.Value));
            }

            if (!string.IsNullOrEmpty(queryParams))
            {
                queryParams = queryParams.Substring(1);
            }

            Uri finalUri;

            if (method == HttpMethod.Get)
            {
                finalUri =
                    new Uri(
                        !string.IsNullOrEmpty(queryParams)
                            ? string.Format(
                                CultureInfo.InvariantCulture,
                                "{0}?{1}",
                                uri.GetLeftPart(UriPartial.Path),
                                queryParams)
                            : uri.GetLeftPart(UriPartial.Path));
            }
            else
            {
                finalUri = new Uri(uri.GetLeftPart(UriPartial.Path));
            }

            HttpWebRequest request = WebRequest.Create(finalUri) as HttpWebRequest;
            // ReSharper disable once PossibleNullReferenceException
            request.Method = HttpMethodConverter.Parse(method);
            request.Accept = baseRequest.Headers["Accept"] ?? "*/*";
            request.UserAgent = baseRequest.UserAgent ?? string.Empty;
            request.CookieContainer = null; // We don't allow any cookies
            request.AllowAutoRedirect = true; // Auto-redirect 3xx responses

            foreach (string header in RequestHeaders)
            {
                AddRequestHeader(request, baseRequest, header);
            }

            // If-Modified-Since header. DateTime parameter, it must be parsed
            if (baseRequest.Headers["If-Modified-Since"] != null)
            {
                DateTime ifModifiedSince;
                DateTime.TryParseExact(
                    request.Headers["If-Modified-Since"],
                    ValidDateFormats,
                    CultureInfo.InvariantCulture.DateTimeFormat,
                    DateTimeStyles.AssumeUniversal,
                    out ifModifiedSince);

                request.IfModifiedSince = ifModifiedSince;
            }

            // Change referer to remove WiseNet Proxy URL
            if (baseRequest.UrlReferrer != null)
            {
                string referer = GetWiseNetBaseReferer(baseRequest, browserUri);
                if (referer != null)
                {
                    request.Referer = referer;
                }
            }

            // If possible, accept compressed data
            request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            // Include POST data in request
            if (method == HttpMethod.Post)
            {
                if (!string.IsNullOrEmpty(queryParams))
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = queryParams.Length;

                    using (Stream formStream = request.GetRequestStream())
                    {
                        byte[] formBytes = new ASCIIEncoding().GetBytes(queryParams);
                        formStream.Write(formBytes, 0, formBytes.Length);
                    }
                }
            }

            return request;
        }

        public static string GetWiseNetBaseReferer(HttpRequestBase baseRequest, Uri browserUri)
        {
            // ReSharper disable once PossibleNullReferenceException
            string referer = baseRequest.UrlReferrer.AbsoluteUri;
            Uri refererUri = new Uri(referer, UriKind.Absolute);

            if (refererUri.GetLeftPart(UriPartial.Authority) == browserUri.GetLeftPart(UriPartial.Authority))
            {
                HttpRequest request = new HttpRequest(null, referer, null);
                HttpResponse response = new HttpResponse(null);
                RouteData routeData =
                    RouteTable.Routes.GetRouteData(new HttpContextWrapper(new HttpContext(request, response)));

                // ReSharper disable once PossibleNullReferenceException
                object routeUri = routeData.Values["uri"];

                if ((routeUri as UrlParameter) == UrlParameter.Optional)
                {
                    referer = null;
                }
                else
                {
                    string routeUriStr = routeUri as string;
                    if (string.IsNullOrWhiteSpace(routeUriStr))
                    {
                        referer = null;
                    }
                    else
                    {
                        Uri uri = new Uri(
                            Encoding.Default.GetString(Convert.FromBase64String(routeUriStr.Replace('-', '='))),
                            UriKind.Absolute);
                        referer = uri.Scheme == "wisenet" ? null : uri.AbsoluteUri;
                    }
                }
            }
            else
            {
                referer = null;
            }

            return referer;
        }

        private static HtmlParser GetParserForUri(Uri uri)
        {
            int? parser;

            using (
                SqlCommand command = SqlDatabaseManager.CreateCommand(
                    CommandType.StoredProcedure,
                    "[dbo].[WISENET_GetParser]"))
            {
                SqlDatabaseManager.AddParameter(
                    command,
                    "@authority",
                    ParameterDirection.Input,
                    SqlDbType.NVarChar,
                    uri.GetLeftPart(UriPartial.Authority));

                parser = SqlDatabaseManager.ExecuteScalar<int?>(command, SqlConnectionString.DataWarehouse);
            }

            return parser.HasValue && Enum.IsDefined(typeof(HtmlParser), parser.Value)
                       ? (HtmlParser)parser.Value
                       : HtmlParser.Default;
        }

        private static void AddRequestHeader(HttpWebRequest request, HttpRequestBase baseRequest, string header)
        {
            string headerValue = baseRequest.Headers[header];
            if (!string.IsNullOrEmpty(headerValue))
            {
                request.Headers.Add(header, headerValue);
            }
        }

        private static string ParseUri(string attrUri, Uri baseUri, HttpRequestBase baseRequest, HtmlParser parser, string method = null)
        {
            Uri uri;

            try
            {
                uri = new Uri(HttpUtility.HtmlDecode(attrUri), UriKind.RelativeOrAbsolute);
            }
            catch
            {
                return null;
            }

            if (!uri.IsAbsoluteUri)
            {
                uri = new Uri(baseUri, uri);
            }

            return ParseUri(uri, baseRequest, parser);
        }

        private static string ParseUri(Uri uri, HttpRequestBase baseRequest, HtmlParser parser, string method = null)
        {
            string encodedUri = ParseUri(uri);
            if (encodedUri == null)
            {
                return null;
            }

            RouteValueDictionary routeValue = new RouteValueDictionary();
            routeValue.Add("parser", (int)parser);

            if (method != null)
            {
                routeValue.Add("method", method);
            }

            VirtualPathData virtualPath = RouteTable.Routes.GetVirtualPathForArea(
                baseRequest.RequestContext,
                "WiseNet_proxy",
                routeValue);

            Uri actionUri = new Uri(virtualPath.VirtualPath, UriKind.Relative);
            // ReSharper disable once PossibleNullReferenceException
            Uri authorityUri = new Uri(baseRequest.Url.GetLeftPart(UriPartial.Authority), UriKind.Absolute);
            UriBuilder browserUri = new UriBuilder(new Uri(authorityUri, actionUri)) { Query = "uri=" + encodedUri };
            return browserUri.Uri.AbsoluteUri;
        }

        private static string ParseUri(Uri uri)
        {
            if (UriSchemeConverter.Convert(uri.Scheme) == UriScheme.NotSupported)
            {
                return null;
            }

            string finalUri, finalUriHash;
            int indexOfHash = uri.AbsoluteUri.IndexOf('#');

            if (indexOfHash > -1)
            {
                finalUri = uri.AbsoluteUri.Substring(0, indexOfHash);
                finalUriHash = uri.AbsoluteUri.Substring(
                    indexOfHash,
                    uri.AbsoluteUri.Length - indexOfHash);
            }
            else
            {
                finalUri = uri.AbsoluteUri;
                finalUriHash = string.Empty;
            }

            return Convert.ToBase64String(Encoding.Default.GetBytes(finalUri)).Replace('=', '-').Replace("/", "_")
                   + finalUriHash;
        }

        #region CSS

        private static string ParseCssUris(string stylesheet, Uri uri, HttpRequestBase baseRequest)
        {
#if !WISENET_ALBACSS
            // By default, we are parsing CSS with very hacky and nasty regexps. In some
            // cases it probably can f*** off the entire stylesheet, but here we are...
            const RegexOptions RegexOptions = RegexOptions.IgnoreCase | RegexOptions.Singleline;

            // I'm pretty sure this crashes if URL has comments like:
            // url("http://www.ietf.org/images/ietfl/* "THIS IS A COMMENT" */ogotrans.gif");
            // Pretty sure? Heh, even VS2013 can't parse it...
            string parsedSheet = Regex.Replace(
                stylesheet,
                @"(url\s*\(\s*)(['""])?((?(2)[^(\2)]+?|[^\)]+?))((?(2)(\2))\s*\))",
                m => CreateCssAbsoluteUri(m, uri, baseRequest),
                RegexOptions);

            parsedSheet = Regex.Replace(
                parsedSheet,
                @"(@import\s*)(['""])([^(\2)]+?)(\2)",
                m => CreateCssAbsoluteUri(m, uri, baseRequest),
                RegexOptions);

            return parsedSheet;
#else
            // TODO: Testing a CSS parser, which obviously is a better option than regexps.
            // Forked version of Alba.CsCss v1.0.1.0 (24 May 2014): https://github.com/Athari/CsCss
            
            return new CssModifier().ModifyUris(stylesheet, url, s => new Uri(url, s).ToString());
#endif
        }

#if !WISENET_ALBACSS

        private static string CreateCssAbsoluteUri(Match m, Uri uri, HttpRequestBase baseRequest)
        {
            if (m.Groups[3].Value.StartsWith("data", StringComparison.InvariantCulture))
            {
                return m.Groups[1].Value + m.Groups[2].Value + m.Groups[3].Value + m.Groups[4].Value;
            }

            Uri attrUri = new Uri(m.Groups[3].Value, UriKind.RelativeOrAbsolute);

            if (!attrUri.IsAbsoluteUri)
            {
                attrUri = new Uri(uri, attrUri);
            }

            return
                m.Groups[1].Value +
                m.Groups[2].Value +
                ParseUri(attrUri, baseRequest, HtmlParser.Default) + 
                m.Groups[4].Value;
        }

#endif

        #endregion

        #endregion

        private static void FixHtmlValueCollection(NameValueCollection nvc)
        {
            if (nvc.AllKeys.Any(x => x == null))
            {
                string[] value = nvc[null].Split(',');

                foreach (string key in value)
                {
                    if (nvc.AllKeys.All(x => x != key))
                    {
                        nvc[key] = null;
                    }
                }

                nvc.Remove(null);
            }
        }
    }
}
