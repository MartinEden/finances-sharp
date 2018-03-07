using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FinancesSharp.Models
{
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
                Categories = Categories.OrderBy(x => x.Name).Select(x => x.Flatten())
            };
        }
    }
}