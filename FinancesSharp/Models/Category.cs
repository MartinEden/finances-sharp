using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using FinancesSharp.Helpers;
using System.Linq.Expressions;

namespace FinancesSharp.Models
{
    public class Category : IHasId, ISelectable
    {
        protected Category()
        {
            DirectChildren = new List<Category>();
            BudgetItems = new List<BudgetItem>();
        }
        public Category(string name, Category parent)
            : this()
        {
            this.Name = name;
            this.Parent = parent;
        }
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public virtual Category Parent { get; set; }
        public virtual IList<BudgetItem> BudgetItems { get; set; }
        public virtual IList<Category> DirectChildren { get; set; }
        public IEnumerable<Category> Children
        {
            get
            {
                yield return this;
                foreach (var child in DirectChildren.SelectMany(c => c.Children).Distinct())
                {
                    yield return child;
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

        internal static void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                        .HasOptional(c => c.Parent)
                        .WithMany(c => c.DirectChildren);
        }

        public object Value
        {
            get { return Id; }
        }

        public object Text
        {
            get { return Name; }
        }

        public Expression<Func<Transaction, bool>> RecursiveFilter
        {
            get
            {
                int[] cats = Children.Select(c => c.Id).ToArray();
                return trx => cats.Contains(trx.Category.Id);
            }
        }
    }
}