using FinancesSharp.Controllers;
using FinancesSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace FinancesSharp.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static FinanceDb Db(this HtmlHelper html)
        {
            return ((IFinancesController)html.ViewContext.Controller).Db;
        }

        public static MvcHtmlString SmartDropdownFor<TModel, TResult>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TResult>> expression, IEnumerable<TResult> options, string empty = " - Select - ")
            where TResult : ISelectable
        {
            return html.EditorFor(expression, "Dropdown", new { options = options, empty = empty });
        }

        public static MvcHtmlString SelectedIf(this HtmlHelper html, bool predicate)
        {
            if (predicate)
            {
                return MvcHtmlString.Create("selected=\"selected\"");
            }
            return MvcHtmlString.Empty;
        }
    }
}