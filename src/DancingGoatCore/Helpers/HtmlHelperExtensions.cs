﻿using System;
using System.Linq.Expressions;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace DancingGoat.Helpers
{
    /// <summary>
    /// Extension methods for <see cref="IHtmlHelper"/> class.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Returns an HTML input element with a label and validation fields for each property in the object that is represented by the <see cref="Expression"/> expression.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="expression">An expression that identifies the object that contains the displayed properties.</param>
        /// <param name="explanationText">An explanation text describing usage of the rendered field.</param>
        /// <param name="disabled">Indicates that field has to be disabled.</param>
        public static IHtmlContent ValidatedEditorFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, LocalizedHtmlString explanationText = null, bool disabled = false)
        {
            var label = html.LabelFor(expression);
            var additionalViewData = disabled ? new { htmlAttributes = new { disabled = "disabled" } } : null;
            var editor = html.EditorFor(expression, additionalViewData);
            var message = html.ValidationMessageFor(expression);
            IHtmlContent explanationTextHtml = HtmlString.Empty;

            if (explanationText != null)
            {
                var explanationDiv = new TagBuilder("div");
                explanationDiv.AddCssClass("explanation-text");
                explanationDiv.InnerHtml.AppendHtml(explanationText);
                explanationDiv.RenderEndTag();
                explanationTextHtml = explanationDiv;
            }

            var generatedHtml = new HtmlContentBuilder().AppendFormat(@"
<div class=""form-group"">
    <div class=""form-group-label"">{0}</div>
    <div class=""form-group-input"">{1}
       {2}
    </div>
    <div class=""message message-error"">{3}</div>
</div>", label, editor, explanationTextHtml, message);

            return  generatedHtml;
        }
    }
}