using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace Heracles.Web
{
    public static class AlteaMvcExtensions
    {
        #region Button

        public static MvcHtmlString Button(this HtmlHelper html)
        {
            return Button(html, submitText: null);
        }

        public static MvcHtmlString Button(this HtmlHelper html, string submitText)
        {
            return Button(html, submitText: submitText, htmlAttributes: null);
        }

        public static MvcHtmlString Button(this HtmlHelper html, object htmlAttributes)
        {
            return Button(html, submitText: null, htmlAttributes: htmlAttributes);
        }

        public static MvcHtmlString Button(this HtmlHelper html, IDictionary<string, object> htmlAttributes)
        {
            return Button(html, submitText: null, htmlAttributes: htmlAttributes);
        }

        public static MvcHtmlString Button(this HtmlHelper html, string submitText, object htmlAttributes)
        {
            return Button(html, submitText: submitText, htmlAttributes: HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString Button(this HtmlHelper html, string submitText, IDictionary<string, object> htmlAttributes)
        {
            return ButtonHelper(html, submitText: submitText, htmlAttributes: htmlAttributes);
        }

        internal static MvcHtmlString ButtonHelper(HtmlHelper html, string submitText = null, IDictionary<string, object> htmlAttributes = null)
        {
            string resolvedSubmitText = submitText ?? String.Empty;

            TagBuilder tag = new TagBuilder("input");
            tag.Attributes.Add("type", "button");

            if (submitText != null)
            {
                tag.Attributes.Add("value", resolvedSubmitText);
            }

            tag.MergeAttributes(htmlAttributes, replaceExisting: true);
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

    #endregion



        #region Submit

        public static MvcHtmlString Submit(this HtmlHelper html)
        {
            return Submit(html, submitText: null);
        }

        public static MvcHtmlString Submit(this HtmlHelper html, string submitText)
        {
            return Submit(html, submitText: submitText, htmlAttributes: null);
        }

        public static MvcHtmlString Submit(this HtmlHelper html, object htmlAttributes)
        {
            return Submit(html, submitText: null, htmlAttributes: htmlAttributes);
        }

        public static MvcHtmlString Submit(this HtmlHelper html, IDictionary<string, object> htmlAttributes)
        {
            return Submit(html, submitText: null, htmlAttributes: htmlAttributes);
        }

        public static MvcHtmlString Submit(this HtmlHelper html, string submitText, object htmlAttributes)
        {
            return Submit(html, submitText: submitText, htmlAttributes: HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString Submit(this HtmlHelper html, string submitText, IDictionary<string, object> htmlAttributes)
        {
            return SubmitHelper(html, submitText: submitText, htmlAttributes: htmlAttributes);
        }

        internal static MvcHtmlString SubmitHelper(HtmlHelper html, string submitText = null, IDictionary<string, object> htmlAttributes = null)
        {
            string resolvedSubmitText = submitText ?? String.Empty;

            TagBuilder tag = new TagBuilder("input");
            tag.Attributes.Add("type", "submit");

            if (submitText != null)
            {
                tag.Attributes.Add("value", resolvedSubmitText);
            }

            tag.MergeAttributes(htmlAttributes, replaceExisting: true);
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        #endregion



        #region CustomCheckBox

        public static MvcHtmlString CustomCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression)
        {
            return CustomCheckBoxFor(htmlHelper, expression, htmlAttributes: null, labelAttributes: null);
        }

        public static MvcHtmlString CustomCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, object htmlAttributes, object labelAttributes)
        {
            return CustomCheckBoxFor(htmlHelper, expression, htmlAttributes: HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), labelAttributes: HtmlHelper.AnonymousObjectToHtmlAttributes(labelAttributes));
        }

        public static MvcHtmlString CustomCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, IDictionary<string, object> htmlAttributes, IDictionary<string, object> labelAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
 
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            bool? isChecked = null;
            if (metadata.Model != null)
            {
                bool modelChecked;
                if (Boolean.TryParse(metadata.Model.ToString(), out modelChecked))
                {
                    isChecked = modelChecked;
                }
            }
            return CustomCheckBoxHelper(htmlHelper, metadata, ExpressionHelper.GetExpressionText(expression), isChecked, htmlAttributes, labelAttributes);
        }

        internal static MvcHtmlString CustomCheckBoxHelper(HtmlHelper htmlHelper, ModelMetadata metadata, string name, bool? isChecked, IDictionary<string, object> htmlAttributes, IDictionary<string, object> labelAttributes)
        {
            RouteValueDictionary attributes = ToRouteValueDictionary(htmlAttributes);

            bool explicitValue = isChecked.HasValue;
            if (explicitValue)
            {
                attributes.Remove("checked"); // Explicit value must override dictionary
            }

            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName))
            {
                throw new ArgumentNullException(name);
            }

            TagBuilder tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.CheckBox));
            tagBuilder.MergeAttribute("name", fullName, true);

            string valueParameter = htmlHelper.FormatValue("true", null);
            bool usedModelState = false;

            bool? modelStateWasChecked = GetModelStateValue(htmlHelper, fullName, typeof(bool)) as bool?;
            if (modelStateWasChecked.HasValue)
            {
                isChecked = modelStateWasChecked.Value;
                usedModelState = true;
            }

            if (!usedModelState)
            {
                string modelStateValue = GetModelStateValue(htmlHelper, fullName, typeof(string)) as string;
                if (modelStateValue != null)
                {
                    isChecked = String.Equals(modelStateValue, valueParameter, StringComparison.Ordinal);
                    usedModelState = true;
                }
            }
            if (!usedModelState && true)
            {
                isChecked = EvalBoolean(htmlHelper, fullName);
            }

            if (isChecked ?? false)
            {
                tagBuilder.MergeAttribute("checked", "checked");
            }

            tagBuilder.MergeAttribute("value", valueParameter, false);

            tagBuilder.GenerateId(fullName);

            StringBuilder inputItemBuilder = new StringBuilder();
            inputItemBuilder.Append(tagBuilder.ToString(TagRenderMode.SelfClosing));

            TagBuilder label = new TagBuilder("label");
            label.MergeAttributes(labelAttributes);
            label.Attributes.Add("for", TagBuilder.CreateSanitizedId(fullName));
            inputItemBuilder.Append(label.ToString(TagRenderMode.Normal));

            return MvcHtmlString.Create(inputItemBuilder.ToString());
        }

        private static RouteValueDictionary ToRouteValueDictionary(IDictionary<string, object> dictionary)
        {
            return dictionary == null ? new RouteValueDictionary() : new RouteValueDictionary(dictionary);
        }

        internal static object GetModelStateValue(HtmlHelper htmlHelper, string key, Type destinationType)
        {
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
            {
                if (modelState.Value != null)
                {
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
                }
            }

            return null;
        }

        internal static bool EvalBoolean(HtmlHelper htmlHelper, string key)
        {
            return Convert.ToBoolean(htmlHelper.ViewData.Eval(key), CultureInfo.InvariantCulture);
        }

        #endregion
    }
}