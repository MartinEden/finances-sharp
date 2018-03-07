using System.Collections.Generic;
using System.Linq;
using FinancesSharp.Models;

namespace FinancesSharp.ViewModels
{
    public class BudgetViewModel
    {
        public Budget Budget { get; }
        public IEnumerable<Category> SpareCategories { get; }

        public BudgetViewModel() {}

        public BudgetViewModel(Budget budget, FinanceDb db)
        {
            Budget = budget;
            SpareCategories = budget.SpareCategories(db);
        }

        public object Flatten()
        {
            return new
            {
                Budget = Budget.Flatten(),
                SpareCategories = SpareCategories.OrderBy(x => x.Name).Select(x => x.Flatten())
            };
        }
        
    }
}