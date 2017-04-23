using FinancesSharp.ViewModels;
using System.Web.Mvc;

namespace FinancesSharp.Binders
{
    public class CategorisationFormBinder : Binder<CategorisationForm>
    {
        protected override void bindProperties(ControllerContext c, ModelBindingContext b)
        {
            bindNavigationProperty(x => x.Category, c, b);
            bindProperty(x => x.CreateRule, c, b);
        }
    }
}