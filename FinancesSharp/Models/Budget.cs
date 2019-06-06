using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FinancesSharp.Models
{
    public class Budget
    {
        public Budget()
        {
            Items = new List<BudgetItem>();
        }
        
        [Key] 
        public int Id { get; set; }
        public virtual IList<BudgetItem> Items { get; set; }
        public decimal MiscellaneousBudget { get; set; }

        public object Flatten()
        {
            return new
            {
                Id,
                MiscellaneousBudget,
                Items = Items.OrderByDescending(x => x.BudgetedAmount).Select(x => x.Flatten())
            };
        }

        public IEnumerable<Category> AllCategories
        {
            get { return Items.SelectMany(x => x.Categories).Distinct(); }
        }

        public IEnumerable<Category> SpareCategories(FinanceDb db)
        {
            return db.Categories
                .ToList()
                .Except(AllCategories)
                .Except(db.Categories.Single(x => x.Name == "Income").Children)
                .ToList();
        }

        public decimal TotalBudget
        {
            get { return Items.Sum(x => x.BudgetedAmount) + MiscellaneousBudget; }
        }
    }
}