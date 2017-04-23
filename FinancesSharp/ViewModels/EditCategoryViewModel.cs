using FinancesSharp.Models;
using System.Web.Mvc;

namespace FinancesSharp.ViewModels
{
    [ModelBinder(typeof(Binder))]
    public class EditCategoryViewModel
    {
        public Category Category { get; set; }
        public Category NewParent { get; set; }
        public string Message { get; private set; }

        public EditCategoryViewModel() { }
        public EditCategoryViewModel(string message)
        {
            Message = message;
        }

        public class Binder : Binders.Binder<EditCategoryViewModel>
        {
            protected override void bindProperties(ControllerContext c, ModelBindingContext b)
            {
                bindNavigationProperty(m => m.Category, c, b);
                bindNavigationProperty(m => m.NewParent, c, b);
            }
        }
    }
}