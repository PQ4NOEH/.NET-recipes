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

    using AngleSharp;
    using AngleSharp.Dom;
    using AngleSharp.Linq;

    using Heracles.Net;

    public partial class WiseNetService
    {
        // ReSharper disable once UnusedMember.Local
        // Only used via reflection
        private static string AngleSharp_RenderHtmlDocument(
            HttpWebResponse response,
            BlacklistData blacklist,
            Uri uri,
            HttpRequestBase baseRequest,
            HtmlParser parser,
            bool isDeveloper)
        {
            IDocument document;

            using (Stream stream = response.GetResponseStream())
            using (TextReader reader = new StreamReader(stream))
            {
                string content = reader.ReadToEnd();
                document = DocumentBuilder.Html(content, null, uri.AbsoluteUri);
            }

            IElement headTag = GetHeadNode(document);

            AngleSharp_RemoveBlacklistData(document, blacklist, headTag);

            AngleSharp_ModifyMetaRefresh(document, uri, baseRequest, parser);
            AngleSharp_InjectAttributes(document, uri, "action", baseRequest, parser);
            AngleSharp_InjectAttributes(document, uri, "href", baseRequest, parser);
            AngleSharp_InjectAttributes(document, uri, "src", baseRequest, parser);
            AngleSharp_UpdateForms(document, uri, baseRequest, parser);
            
            // Inject Styles
            IHtmlCollection styles = document.QuerySelectorAll(@"style");

            foreach (IElement node in styles)
            {
                string css = node.Text();

                if (css == null)
                {
                    continue;
                }

                node.InnerHtml = ParseCssUris(css, uri, baseRequest);
            }
            
            // Inject Style Attributes
            IHtmlCollection styleAttributes = document.QuerySelectorAll(@"*[style]");

            foreach (IElement node in styleAttributes)
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
                AngleSharp_SetBaseTag(document, headTag, uri.AbsoluteUri);

                if (!isDeveloper)
                {
                    AngleSharp_AddForceWiseNetScript(document, headTag, uri, baseRequest, parser);
                }
            }

            return document.ToHtml();
        }

        private static IElement GetHeadNode(IDocument document)
        {
            if (document.DocumentElement.OuterHtml == null)
            {
                return null;
            }

            IHtmlCollection headTags = document.GetElementsByTagName("head");
            IElement headTag;

            if (headTags.Length == 0)
            {
                // No HEAD tag
                headTag = document.CreateElement("head");

                IElement firstNode = document.FirstChild as IElement;

                if (firstNode != null && firstNode.TagName == "html")
                {
                    firstNode.Prepend(headTag);
                }
                else
                {
                    document.DocumentElement.Prepend(headTag);
                }
            }
            else
            {
                headTag = headTags[0];
            }

            return headTag;
        }

        #region Blacklist

        private static void AngleSharp_RemoveBlacklistData(IDocument document, BlacklistData blacklist, IElement headTag)
        {
            // Strength: HREF and SRC attributes with JavaScript content
            if (blacklist.Strength.HasFlag(BlacklistStrength.SourcesAndReferences))
            {
                AngleSharp_RemoveSourcesAndReferences(document);
            }

            // Strength: events (onclick, onfocus...)
            if (blacklist.Strength.HasFlag(BlacklistStrength.TagEventAttributes))
            {
                AngleSharp_RemoveEventAttributes(document);
            }

            // Strength: script and noscript tags
            if (blacklist.Strength.HasFlag(BlacklistStrength.ScriptAndNoscriptTags))
            {
                AngleSharp_RemoveScriptTags(document);
            }

            // Remove Tags
            AngleSharp_RemoveTags(document, blacklist.RemoveTags);

            // Remove Tag Attributes
            AngleSharp_RemoveTagAttributes(document, blacklist.RemoveAttributes);

            // Inject Head Code
            AngleSharp_InjectHeadCode(document, headTag, blacklist.InjectHeadCode);
        }

        private static void AngleSharp_RemoveSourcesAndReferences(IDocument document)
        {
            IHtmlCollection scriptLinksHref =
                document.QuerySelectorAll(@"*[href^=""javascript""], *[href^=""jscript""], *[href^=""vbscript""]");

            foreach (IElement node in scriptLinksHref)
            {
                node.RemoveAttribute("href");
            }

            IHtmlCollection scriptLinksSrc =
                document.QuerySelectorAll(@"*[src^=""javascript""], *[src^=""jscript""], *[src^=""vbscript""]");

            foreach (IElement node in scriptLinksSrc)
            {
                node.RemoveAttribute("src");
            }
        }

        private static void AngleSharp_RemoveEventAttributes(IDocument document)
        {
            IHtmlCollection eventLinks =
                document.QuerySelectorAll(
                    @"*[onclick], *[onmouseover], *[onfocus], *[onblur], *[onmouseout], *[ondoubleclic], *[onload], *[onunload]");

            foreach (IElement node in eventLinks)
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

        private static void AngleSharp_RemoveScriptTags(IDocument document)
        {
            IHtmlCollection scripts = document.QuerySelectorAll(@"script, noscript");

            foreach (IElement node in scripts)
            {
                node.Remove();
            }
        }

        private static void AngleSharp_RemoveTags(IDocument document, IEnumerable<string> tags)
        {
            foreach (string tag in tags)
            {
                IHtmlCollection nodes = document.QuerySelectorAll(tag);

                foreach (IElement node in nodes)
                {
                    node.Remove();
                }
            }
        }

        private static void AngleSharp_RemoveTagAttributes(IDocument document, IEnumerable<string> tagAttributes)
        {
            foreach (string attribute in tagAttributes)
            {
                IHtmlCollection nodes = document.QuerySelectorAll(@"*[" + attribute + @"]");

                foreach (IElement node in nodes)
                {
                    node.RemoveAttribute(attribute);
                }
            }
        }

        private static void AngleSharp_InjectHeadCode(IDocument document, IElement headTag, string code)
        {
            if (headTag != null)
            {
                headTag.Prepend(document.CreateTextNode(code));
            }
        }

        #endregion

        private static void AngleSharp_ModifyMetaRefresh(IDocument document, Uri baseUri, HttpRequestBase baseRequest, HtmlParser parser)
        {
            IHtmlCollection metaRefresh = document.QuerySelectorAll(@"meta[http-equiv=""refresh""][content]");

            foreach (IElement node in metaRefresh)
            {
                string value = node.GetAttribute("content");
                if (value != null)
                {
                    node.SetAttribute("content", ParseUri(value, baseUri, baseRequest, parser));
                }
            }
        }

        private static void AngleSharp_InjectAttributes(IDocument document, Uri baseUri, string attribute, HttpRequestBase baseRequest, HtmlParser parser)
        {
            IHtmlCollection nodes = document.QuerySelectorAll(@"*[" + attribute + @"]");

            foreach (IElement node in nodes)
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

        private static void AngleSharp_UpdateForms(IDocument document, Uri baseUri, HttpRequestBase baseRequest, HtmlParser parser)
        {
            IHtmlCollection nodes = document.QuerySelectorAll(@"form");

            foreach (IElement node in nodes)
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

        private static void AngleSharp_SetBaseTag(IDocument document, IElement headTag, string uri)
        {
            IHtmlCollection tags = document.GetElementsByTagName("base");

            foreach (IElement tag in tags)
            {
                tag.Remove();
            }

            IElement baseTag = document.CreateElement("base");
            baseTag.SetAttribute("href", uri);

            headTag.AppendChild(baseTag);
        }

        private static void AngleSharp_AddForceWiseNetScript(IDocument document, IElement headTag, Uri uri, HttpRequestBase baseRequest, HtmlParser parser)
        {
            IElement scriptTag = document.CreateElement("script");
            scriptTag.SetAttribute("type", "text/javascript");
            scriptTag.InnerHtml = string.Format(
                CultureInfo.InvariantCulture,
                ForceWiseNetScript,
                ParseUri(uri, baseRequest, parser));

            headTag.Prepend(scriptTag);
        }
    }
}
