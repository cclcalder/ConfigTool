using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace SalesPlannerWeb.Helpers
{
    public static class DisplayWithBreaksHelper
    {
        // http://stackoverflow.com/questions/9030763/how-to-make-html-displayfor-display-line-breaks
        public static MvcHtmlString DisplayWithBreaksFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var model = html.Encode(metadata.Model).Replace("\n", "<br />");

            if (String.IsNullOrEmpty(model))
                return MvcHtmlString.Empty;

            return MvcHtmlString.Create(model);
        }
    }
}