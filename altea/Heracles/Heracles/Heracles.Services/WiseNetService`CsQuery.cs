namespace Heracles.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Web;

    using Altea.Classes.WiseNet;

    using CsQuery;
    using CsQuery.Implementation;

    using Heracles.Net;

    public partial class WiseNetService
    {
        // ReSharper disable once UnusedMember.Local
        // Only used via reflection
        private static string CsQuery_RenderHtmlDocument(
            HttpWebResponse response,
            BlacklistData blacklist,
            Uri uri,
            HttpRequestBase baseRequest,
            HtmlParser parser,
            bool isDeveloper)
        {
            CQ document;

            using (Stream stream = response.GetResponseStream())
            {
                document = CQ.CreateDocument(stream);
            }

            DomElement headTag = GetHeadNode(document);

            CsQuery_RemoveBlacklistData(document, blacklist, headTag);

            CsQuery_ModifyMetaRefresh(document, uri, baseRequest, parser);
            CsQuery_InjectAttributes(document, uri, "action", baseRequest, parser);
            CsQuery_InjectAttributes(document, uri, "href", baseRequest, parser);
            CsQuery_InjectAttributes(document, uri, "src", baseRequest, parser);
            CsQuery_UpdateForms(document, uri, baseRequest, parser);

            // Inject Styles
            CQ styles = document.Select(@"style");

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (DomElement node in styles)
            {
                string css = node.InnerHTML;

                if (css == null)
                {
                    continue;
                }

                node.InnerText = ParseCssUris(css, uri, baseRequest);
            }

            // Inject Style Attributes
            CQ styleAttributes = document.Select(@"*[style]");

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (DomElement node in styleAttributes)
            {
                string css = node.GetAttribute("style");

                if (css == null)
                {
                    continue;
                }

                node.SetAttribute("style", ParseCssUris(css, uri, baseRequest));
            }

            if (headTag != null)
            {
                CsQuery_SetBaseTag(document, headTag, uri.AbsoluteUri);

                if (!isDeveloper)
                {
                    CsQuery_AddForceWiseNetScript(document, headTag, uri, baseRequest, parser);
                }
            }

            return document.Render();
        }

        private static DomElement GetHeadNode(CQ document)
        {
            if (document.Length == 0)
            {
                return null;
            }

            CQ headTags = document.Select("head");
            DomElement headTag;

            if (headTags.Length == 0)
            {
                // No HEAD tag
                headTag = (DomElement)document.Document.CreateElement("head");
                DomElement firstNode = document.FirstElement() as DomElement;

                if (firstNode.Name == "html")
                {
                    if (firstNode.HasChildren)
                    {
                        firstNode.InsertBefore(headTag, firstNode.FirstChild);
                    }
                    else
                    {
                        firstNode.AppendChild(headTag);
                    }
                }
                else
                {
                    document.Document.InsertBefore(headTag, document.Document.FirstChild);
                }
            }
            else
            {
                headTag = (DomElement)headTags.FirstElement();
            }

            return headTag;
        }

        #region Blacklist

        private static void CsQuery_RemoveBlacklistData(CQ document, BlacklistData blacklist, DomElement headTag)
        {
            // Strength: HREF and SRC attributes with JavaScript Content
            if (blacklist.Strength.HasFlag(BlacklistStrength.SourcesAndReferences))
            {
                CsQuery_RemoveSourcesAndReferences(document);
            }

            // Strength: events (onclick, onfocus...)
            if (blacklist.Strength.HasFlag(BlacklistStrength.TagEventAttributes))
            {
                CsQuery_RemoveEventAttributes(document);
            }

            // Strength: script and noscript tags
            if (blacklist.Strength.HasFlag(BlacklistStrength.ScriptAndNoscriptTags))
            {
                CsQuery_RemoveScriptTags(document);
            }

            // Remove Tags
            CsQuery_RemoveTags(document, blacklist.RemoveTags);

            // Remove Tag Attributes
            CsQuery_RemoveTagAttributes(document, blacklist.RemoveAttributes);

            // Inject Head Code
            CsQuery_InjectHeadCode(document, headTag, blacklist.InjectHeadCode);
        }

        private static void CsQuery_RemoveSourcesAndReferences(CQ document)
        {
            CQ scriptLinksHref = document.Select(@"*[href^=""javascript""], *[href^=""jscript""], *[href^=""vbscript""]");

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (DomElement node in scriptLinksHref)
            {
                node.RemoveAttribute("href");
            }

            CQ scriptLinksSrc = document.Select(@"*[src^=""javascript""], *[src^=""jscript""], *[src^=""vbscript""]");

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (DomElement node in scriptLinksSrc)
            {
                node.RemoveAttribute("src");
            }
        }

        private static void CsQuery_RemoveEventAttributes(CQ document)
        {
            CQ eventLinks =
                document.Select(
                    @"*[onclick], *[onmouseover], *[onfocus], *[onblur], *[onmouseout], *[ondoubleclic], *[onload], *[onunload]");

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (DomElement node in eventLinks)
            {
                node.RemoveAttribute("onClick");
                node.RemoveAttribute("onMouseOver");
                node.RemoveAttribute("onFocus");
                node.RemoveAttribute("onBlur");
                node.RemoveAttribute("onMouseOut");
                node.RemoveAttribute("onDoubleClick");
                node.RemoveAttribute("onLoad");
                node.RemoveAttribute("onUnload");
            }
        }

        private static void CsQuery_RemoveScriptTags(CQ document)
        {
            CQ scripts = document.Select(@"script, noscript");

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (DomElement node in scripts)
            {
                node.Remove();
            }
        }

        private static void CsQuery_RemoveTags(CQ document, IEnumerable<string> tags)
        {
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            // Code is not readable if converted to LINQ
            foreach (string tag in tags)
            {
                CQ nodes = document.Select(tag);

                // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                foreach (DomElement node in nodes)
                {
                    node.Remove();
                }
            }
        }

        private static void CsQuery_RemoveTagAttributes(CQ document, IEnumerable<string> tagAttributes)
        {
            foreach (string attribute in tagAttributes)
            {
                CQ nodes = document.Select(@"*[" + attribute + @"]");

                // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                foreach (DomElement node in nodes)
                {
                    node.RemoveAttribute(attribute);
                }
            }
        }

        private static void CsQuery_InjectHeadCode(CQ document, DomElement headTag, string code)
        {
            if (headTag != null)
            {
                IDomText text = document.Document.CreateTextNode(code);

                if (headTag.HasChildren)
                {
                    headTag.InsertBefore(text, headTag.FirstChild);
                }
                else
                {
                    headTag.AppendChild(text);
                }
            }
        }

        #endregion

        private static void CsQuery_ModifyMetaRefresh(
            CQ document,
            Uri baseUri,
            HttpRequestBase baseRequest,
            HtmlParser parser)
        {
            CQ metaRefresh = document.Select(@"meta[http-equiv=""refresh""][content]");

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (DomElement node in metaRefresh)
            {
                string value = node.GetAttribute("content");
                if (value != null)
                {
                    node.SetAttribute("content", ParseUri(value, baseUri, baseRequest, parser));
                }
            }
        }

        private static void CsQuery_InjectAttributes(
            CQ document,
            Uri baseUri,
            string attribute,
            HttpRequestBase baseRequest,
            HtmlParser parser)
        {
            CQ nodes = document.Select(@"*[" + attribute + @"]");

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (DomElement node in nodes)
            {
                node.RemoveAttribute("target");
                string attrUri = node.GetAttribute(attribute);

                if (attrUri == null || attrUri.StartsWith("data:") || attrUri.StartsWith("javascript:"))
                {
                    continue;
                }

                string fullFinalUri = ParseUri(attrUri, baseUri, baseRequest, parser);

                if (fullFinalUri == null)
                {
                    continue;
                }

                node.SetAttribute(attribute, fullFinalUri);
                node.SetAttribute(InjectedHtmlAttribute, InjectedHtmlAttributeValue);
            }
        }

        private static void CsQuery_UpdateForms(CQ document, Uri baseUri, HttpRequestBase baseRequest, HtmlParser parser)
        {
            CQ nodes = document.Select(@"form");

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (DomElement node in nodes)
            {
                string action = node.GetAttribute("action");
                string method = node.GetAttribute("method");
                HttpMethod httpMethod = HttpMethodConverter.Convert(method);
                if (httpMethod == HttpMethod.NotSupported)
                {
                    httpMethod = HttpMethod.Get;
                }

                node.SetAttribute("method", "post");

                if (action == null)
                {
                    action = ParseUri(baseUri, baseRequest, parser, HttpMethodConverter.Parse(httpMethod));
                    node.SetAttribute("action", action);

                }
                else if (!action.StartsWith("javascript:"))
                {
                    UriBuilder builder = new UriBuilder(action);
                    NameValueCollection query = HttpUtility.ParseQueryString(builder.Query);
                    query["method"] = HttpMethodConverter.Parse(httpMethod);
                    builder.Query = query.ToString();
                    node.SetAttribute("action", builder.Uri.AbsoluteUri);
                }
            }
        }

        private static void CsQuery_SetBaseTag(CQ document, DomElement headTag, string uri)
        {
            CQ tags = document.Select("base");

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (DomElement tag in tags)
            {
                tag.Remove();
            }

            DomElement baseTag = (DomElement)document.Document.CreateElement("base");
            baseTag.SetAttribute("href", uri);


            headTag.AppendChild(baseTag);
        }

        private static void CsQuery_AddForceWiseNetScript(
            CQ document,
            DomElement headTag,
            Uri uri,
            HttpRequestBase baseRequest,
            HtmlParser parser)
        {
            HTMLScriptElement scriptTag = (HTMLScriptElement)document.Document.CreateElement("script");
            scriptTag.SetAttribute("type", "text/javascript");
            scriptTag.InnerText = string.Format(
                CultureInfo.InvariantCulture,
                ForceWiseNetScript,
                ParseUri(uri, baseRequest, parser));

            headTag.InsertBefore(scriptTag, headTag.FirstChild);
        }
    }
}
