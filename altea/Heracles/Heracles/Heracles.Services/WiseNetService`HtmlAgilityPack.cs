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

    using Heracles.Net;

    using HtmlAgilityPack;

    public partial class WiseNetService
    {
        // ReSharper disable once UnusedMember.Local
        // Only used via reflection
        private static string HtmlAgilityPack_RenderHtmlDocument(
            WebResponse response,
            BlacklistData blacklist,
            Uri uri,
            HttpRequestBase baseRequest,
            HtmlParser parser,
            bool isDeveloper)
        {
            HtmlDocument document = new HtmlDocument
            {
                OptionAutoCloseOnEnd = false,
                OptionCheckSyntax = false,
                OptionFixNestedTags = false,
                OptionOutputOriginalCase = true,
                OptionWriteEmptyNodes = true
            };

            using (Stream stream = response.GetResponseStream())
            {
                document.Load(stream, true);
            }

            HtmlNode headTag = GetHeadNode(document);

            HtmlAgilityPack_RemoveBlacklistData(document, blacklist, headTag);

            HtmlAgilityPack_ModifyMetaRefresh(document, uri, baseRequest, parser);
            HtmlAgilityPack_InjectAttributes(document, uri, "action", baseRequest, parser);
            HtmlAgilityPack_InjectAttributes(document, uri, "href", baseRequest, parser);
            HtmlAgilityPack_InjectAttributes(document, uri, "src", baseRequest, parser);
            HtmlAgilityPack_UpdateForms(document, uri, baseRequest, parser);

            // Inject Styles
            HtmlNodeCollection styles = document.DocumentNode.SelectNodes(@"//style");
            if (styles != null)
            {
                foreach (HtmlNode node in styles)
                {
                    string css = node.InnerText;

                    if (css == null)
                    {
                        continue;
                    }

                    node.InnerHtml = ParseCssUris(css, uri, baseRequest);
                }
            }

            // Inject Style Attributes
            HtmlNodeCollection styleAttributes = document.DocumentNode.SelectNodes(@"//*[@style]");
            if (styleAttributes != null)
            {
                foreach (HtmlNode node in styleAttributes)
                {
                    string css = node.GetAttributeValue("style", null);

                    if (css == null)
                    {
                        continue;
                    }

                    node.SetAttributeValue("style", ParseCssUris(css, uri, baseRequest));
                }
            }

            if (headTag != null)
            {
                HtmlAgilityPack_SetBaseTag(document, headTag, uri.AbsoluteUri);

                if (!isDeveloper)
                {
                    HtmlAgilityPack_AddForceWiseNetScript(document, headTag, uri, baseRequest, parser);
                }
            }

            return document.DocumentNode.OuterHtml;
        }

        private static HtmlNode GetHeadNode(HtmlDocument document)
        {
            if (document.DocumentNode.OuterHtml == null)
            {
                return null;
            }

            HtmlNode headTag = document.DocumentNode.SelectSingleNode(@"//head");

            if (headTag == null)
            {
                // No HEAD tag
                headTag = document.CreateElement("head");

                HtmlNode firstNode = document.DocumentNode.SelectSingleNode(@"/");

                if (firstNode.Name == "html")
                {
                    firstNode.PrependChild(headTag);
                }
                else
                {
                    document.DocumentNode.PrependChild(headTag);
                }
            }

            return headTag;
        }

        #region Blacklist

        private static void HtmlAgilityPack_RemoveBlacklistData(HtmlDocument document, BlacklistData blacklist, HtmlNode headTag)
        {
            // Strength: HREF and SRC attributes with JavaScript content
            if (blacklist.Strength.HasFlag(BlacklistStrength.SourcesAndReferences))
            {
                HtmlAgilityPack_RemoveSourcesAndReferences(document);
            }

            // Strength: events (onclick, onfocus...)
            if (blacklist.Strength.HasFlag(BlacklistStrength.TagEventAttributes))
            {
                HtmlAgilityPack_RemoveEventAttributes(document);
            }

            // Strength: script and noscript tags
            if (blacklist.Strength.HasFlag(BlacklistStrength.ScriptAndNoscriptTags))
            {
                HtmlAgilityPack_RemoveScriptTags(document);
            }

            // Remove Tags
            HtmlAgilityPack_RemoveTags(document, blacklist.RemoveTags);
            
            // Remove Tag Attributes
            HtmlAgilityPack_RemoveTagAttributes(document, blacklist.RemoveAttributes);

            // Inject Head Code
            HtmlAgilityPack_InjectHeadCode(document, headTag, blacklist.InjectHeadCode);
        }

        private static void HtmlAgilityPack_RemoveSourcesAndReferences(HtmlDocument document)
        {
            HtmlNodeCollection scriptLinksHref =
                document.DocumentNode.SelectNodes(
                    @"//*[starts-with(@href, 'javascript') or starts-with(@href, 'jscript') or starts-with(@href, 'vbscript')]");

            if (scriptLinksHref != null)
            {
                foreach (HtmlNode node in scriptLinksHref)
                {
                    node.Attributes.Remove("href");
                }
            }

            HtmlNodeCollection scriptLinksSrc =
                document.DocumentNode.SelectNodes(
                    @"//*[starts-with(@src, 'javascript') or starts-with(@src, 'jscript') or starts-with(@src, 'vbscript')]");

            if (scriptLinksSrc != null)
            {
                foreach (HtmlNode node in scriptLinksSrc)
                {
                    node.Attributes.Remove("src");
                }
            }
        }

        private static void HtmlAgilityPack_RemoveEventAttributes(HtmlDocument document)
        {
            HtmlNodeCollection eventLinks =
                document.DocumentNode.SelectNodes(
                    @"//*[@onclick or @onmouseover or @onfocus or @onblur or @onmouseout or @ondoubleclick or @onload or @onunload]");

            if (eventLinks != null)
            {
                foreach (HtmlNode node in eventLinks)
                {
                    node.Attributes.Remove("onClick");
                    node.Attributes.Remove("onMouseOver");
                    node.Attributes.Remove("onFocus");
                    node.Attributes.Remove("onBlur");
                    node.Attributes.Remove("onMouseOut");
                    node.Attributes.Remove("onDoubleClick");
                    node.Attributes.Remove("onLoad");
                    node.Attributes.Remove("onUnload");
                }
            }
        }

        private static void HtmlAgilityPack_RemoveScriptTags(HtmlDocument document)
        {
            HtmlNodeCollection scripts = document.DocumentNode.SelectNodes(@"//script|//noscript");
            if (scripts != null)
            {
                foreach (HtmlNode node in scripts)
                {
                    node.Remove();
                }
            }
        }

        private static void HtmlAgilityPack_RemoveTags(HtmlDocument document, IEnumerable<string> tags)
        {
            foreach (string tag in tags)
            {
                HtmlNodeCollection nodes;

                if (tag[0] == '.')
                {
                    // Class
                    nodes = document.DocumentNode.SelectNodes(@"//*[@class=""" + tag.Substring(1) + @"""]");
                }
                else if (tag[0] == '#')
                {
                    // Id
                    nodes = document.DocumentNode.SelectNodes(@"//*[@id=""" + tag.Substring(1) + @"""]");
                }
                else if (tag.IndexOf('.') != -1)
                {
                    // Tag + Class
                    int indexOf = tag.IndexOf('.');
                    nodes =
                        document.DocumentNode.SelectNodes(
                            @"//" + tag.Substring(0, indexOf) + @"[@class=""" + tag.Substring(indexOf) + @"""]");
                }
                else if (tag.IndexOf('#') != -1)
                {
                    // Tag + Id
                    int indexOf = tag.IndexOf('#');
                    nodes =
                        document.DocumentNode.SelectNodes(
                            @"//" + tag.Substring(0, indexOf) + @"[@id=""" + tag.Substring(indexOf) + @"""]");
                }
                else
                {
                    // Tag
                    nodes = document.DocumentNode.SelectNodes(@"//" + tag);
                }

                foreach (HtmlNode node in nodes)
                {
                    node.Remove();
                }
            }
        }

        private static void HtmlAgilityPack_RemoveTagAttributes(HtmlDocument document, IEnumerable<string> tagAttributes)
        {
            foreach (string attribute in tagAttributes)
            {
                HtmlNodeCollection nodes = document.DocumentNode.SelectNodes(@"//*[@" + attribute + @"]");

                if (nodes != null)
                {
                    foreach (HtmlNode node in nodes)
                    {
                        node.Attributes.Remove(attribute);
                    }
                }
            }
        }

        private static void HtmlAgilityPack_InjectHeadCode(HtmlDocument document, HtmlNode headTag, string code)
        {
            if (headTag != null)
            {
                headTag.PrependChild(document.CreateTextNode(code));
            }
        }

        #endregion

        private static void HtmlAgilityPack_ModifyMetaRefresh(HtmlDocument document, Uri baseUri, HttpRequestBase baseRequest, HtmlParser parser)
        {
            HtmlNodeCollection metaRefresh =
                document.DocumentNode.SelectNodes(
                    @"//meta[translate(@http-equiv,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='refresh' and @content]");

            if (metaRefresh != null)
            {
                foreach (HtmlNode node in metaRefresh)
                {
                    string value = node.GetAttributeValue("content", null);
                    if (value != null)
                    {
                        node.SetAttributeValue("content", ParseUri(value, baseUri, baseRequest, parser));
                    }
                }
            }
        }

        private static void HtmlAgilityPack_InjectAttributes(
            HtmlDocument document,
            Uri baseUri,
            string attribute,
            HttpRequestBase baseRequest,
            HtmlParser parser)
        {
            HtmlNodeCollection nodes = document.DocumentNode.SelectNodes(@"//*[@" + attribute + @"]");

            if (nodes == null)
            {
                return;
            }

            foreach (HtmlNode node in nodes)
            {
                node.Attributes.Remove("target");
                string attrUri = node.GetAttributeValue(attribute, null);

                if (attrUri == null || attrUri.StartsWith("data:") | attrUri.StartsWith("javascript:"))
                {
                    continue;
                }

                string fullFinalUri = ParseUri(attrUri, baseUri, baseRequest, parser);

                if (fullFinalUri == null)
                {
                    continue;
                }

                node.SetAttributeValue(attribute, fullFinalUri);
                node.SetAttributeValue(InjectedHtmlAttribute, InjectedHtmlAttributeValue);
            }
        }

        private static void HtmlAgilityPack_UpdateForms(HtmlDocument document, Uri baseUri, HttpRequestBase baseRequest, HtmlParser parser)
        {
            HtmlNodeCollection nodes = document.DocumentNode.SelectNodes(@"//form");

            foreach (HtmlNode node in nodes)
            {
                string action = node.GetAttributeValue("action", null);
                string method = node.GetAttributeValue("method", null);
                HttpMethod httpMethod = HttpMethodConverter.Convert(method);
                if (httpMethod == HttpMethod.NotSupported)
                {
                    httpMethod = HttpMethod.Get;
                }

                node.SetAttributeValue("method", "post");

                if (action == null)
                {
                    action = ParseUri(baseUri, baseRequest, parser, HttpMethodConverter.Parse(httpMethod));
                    node.SetAttributeValue("action", action);

                }
                else if (!action.StartsWith("javascript:"))
                {
                    UriBuilder builder = new UriBuilder(action);
                    NameValueCollection query = HttpUtility.ParseQueryString(builder.Query);
                    query["method"] = HttpMethodConverter.Parse(httpMethod);
                    builder.Query = query.ToString();
                    node.SetAttributeValue("action", builder.Uri.AbsoluteUri);
                }
            }
        }

        private static void HtmlAgilityPack_SetBaseTag(HtmlDocument document, HtmlNode headTag, string uri)
        {
            HtmlNodeCollection tags = document.DocumentNode.SelectNodes(@"//base");
            
            if (tags != null)
            {
                foreach (HtmlNode tag in tags)
                {
                    tag.Remove();
                }
            }

            HtmlNode baseTag = document.CreateElement("base");
            baseTag.SetAttributeValue("href", uri);

            headTag.AppendChild(baseTag);
        }

        private static void HtmlAgilityPack_AddForceWiseNetScript(
            HtmlDocument document,
            HtmlNode headTag,
            Uri uri,
            HttpRequestBase baseRequest,
            HtmlParser parser)
        {
            HtmlNode scriptTag = document.CreateElement("script");
            scriptTag.SetAttributeValue("type", "text/javascript");
            HtmlTextNode scriptContent =
                document.CreateTextNode(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        ForceWiseNetScript,
                        ParseUri(uri, baseRequest, parser)));
            scriptTag.AppendChild(scriptContent);

            headTag.PrependChild(scriptTag);
        }
    }
}
