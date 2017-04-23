using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FinancesSharp.Models
{
    public class CategorisationRule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public virtual Search Criteria { get; set; }
        [Required]
        public virtual Category Category { get; set; }
        public bool Active { get; set; }

        private CategorisationRule() { }
        public CategorisationRule(Search criteria, Category category)
        {
            Criteria = criteria;
            Category = category;
            Active = true;
        }

        public bool Matches(Transaction trx)
        {
            return Criteria.IsMatch().Compile()(trx);
        }
        public void ApplyTo(Transaction trx)
        {
            trx.Category = Category;
        }

        public override string ToString()
        {
            return string.Format("Categorise as '{0}' when {1} (Active: {2})", Category, Criteria, Active);
        }
    }
}