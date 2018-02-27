using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.UI;

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

        public object Flatten()
        {
            return new
            {
                Id,
                Items = Items.Select(x => x.Flatten())
            };
        }
    }
    
    public class BudgetItem
    {
        public BudgetItem()
        {
            Categories = new List<Category>();
        }
        
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual IList<Category> Categories { get; set; }
        public decimal BudgetedAmount { get; set; }

        public object Flatten()
        {
            return new
            {
                Id,
                Name,
                BudgetedAmount,
                Categories = Categories.Select(x => x.Flatten())
            };
        }
    }
}