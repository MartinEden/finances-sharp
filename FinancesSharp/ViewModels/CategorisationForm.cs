using FinancesSharp.Binders;
using FinancesSharp.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FinancesSharp.ViewModels
{
    [ModelBinder(typeof(CategorisationFormBinder))]
    public class CategorisationForm
    {
        [Required]
        public Category Category { get; set; }
        public bool CreateRule { get; set; }
        public bool ShowSection { get; private set; }

        public void Show()
        {
            ShowSection = true;
        }
    }
}