using FinancesSharp.Models;
using System.Web.Mvc;

namespace FinancesSharp.Binders
{
    public class SearchBinder : Binder<Search>
    {
        protected override void bindProperties(ControllerContext c, ModelBindingContext b)
        {            
            bindProperty(x => x.Name, c, b);
            bindNavigationProperty(x => x.Category, c, b);
            bindProperty(x => x.IncludeSubCategories, c, b);
            bindDate(x => x.DateFrom, c, b);
            bindDate(x => x.DateTo, c, b);
            bindProperty(x => x.Income, c, b);
            bindProperty(x => x.AmountFrom, c, b);
            bindProperty(x => x.AmountTo, c, b);
            bindNavigationProperty(x => x.Person, c, b);
        }
    }
}