using FinancesSharp.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FinancesSharp.ViewModels
{
    public class RulesViewModel
    {
        public IEnumerable<CategorisationRule> Rules { get; private set; }

        public RulesViewModel(FinanceDb db)
        {
            Rules = db.ActiveRules
                .OrderBy(x => x.Category.Id)
                .ToList();
        }
    }
}