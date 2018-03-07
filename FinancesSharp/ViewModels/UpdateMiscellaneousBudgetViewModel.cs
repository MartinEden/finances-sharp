using System.ComponentModel.DataAnnotations;

namespace FinancesSharp.ViewModels
{
    public class UpdateMiscellaneousBudgetViewModel
    {
        [Required, Range(0, int.MaxValue)]
        public decimal MiscellaneousBudget { get; set; }
    }
}