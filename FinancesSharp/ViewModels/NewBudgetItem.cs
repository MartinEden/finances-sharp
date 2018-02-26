using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FinancesSharp.ViewModels
{
    public class NewBudgetItem
    {
        [Required]
        public string Name { get; set; }
        [Required, Range(0, int.MaxValue)]
        [DisplayName("Budgeted amount")]
        public decimal BudgetedAmount { get; set; }
    }
}