using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FinancesSharp.ViewModels
{
    public class AddCategoryToBudgetItem
    {
        [Required, DisplayName("Budget item")]
        public int ItemId { get; set; }
        [Required, DisplayName("Category")]
        public int CategoryId { get; set; }
    }
}