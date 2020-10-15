using FinancesSharp.Binders;
using FinancesSharp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace FinancesSharp.Models
{
    [ModelBinder(typeof(SearchBinder))]
    public class Search
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public virtual Category Category { get; set; }
        [Display(Name = "Include subcategories")]
        public bool IncludeSubCategories { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool? Income { get; set; }
        public Decimal? AmountFrom { get; set; }
        public Decimal? AmountTo { get; set; }
        public virtual Person Person { get; set; }

        public Search()
        {
            IncludeSubCategories = true;
        }

        public Expression<Func<Transaction, bool>> IsMatch()
        {
            Expression<Func<Transaction, bool>> expr = trx => true;
            if (Name != null) { expr = expr.And(trx => trx.Name.ToLower().Contains(Name.ToLower())); }
            if (Category != null)
            {
                if (IncludeSubCategories)
                {
                    expr = expr.And(Category.RecursiveFilter);
                }
                else
                {
                    expr = expr.And(trx => trx.Category.Id == Category.Id);
                }
            }
            if (DateFrom != null) { expr = expr.And(trx => DateFrom <= trx.Date); }
            if (DateTo != null) { expr = expr.And(trx => DateTo >= trx.Date); }
            if (Income != null) { expr = expr.And(trx => Income == trx.Amount > 0); }
            if (AmountFrom != null) { expr = expr.And(trx => AmountFrom <= Math.Abs(trx.Amount)); }
            if (AmountTo != null) { expr = expr.And(trx => AmountTo >= Math.Abs(trx.Amount)); }
            if (Person != null) { expr = expr.And(trx => trx.Card != null && Person.Id == trx.Card.Person.Id); }
            return expr;
        }

        public override string ToString()
        {
            return string.Join(", ", Criteria);
        }
        public IEnumerable<string> Criteria
        {
            get
            {
                if (Name != null)
                {
                    yield return string.Format("Name contains '{0}'", Name);
                }
                if (Category != null)
                {
                    yield return "In category " + Category;
                }
                if (DateFrom != null)
                {
                    yield return "From date " + DateFrom.Value.ToShortDateString();
                }
                if (DateTo != null)
                {
                    yield return "To date" + DateTo.Value.ToShortDateString();
                }
                if (Income != null)
                {
                    if (Income.Value)
                    {
                        yield return "Positive income";
                    }
                    else
                    {
                        yield return "Negative income";
                    }
                }
                if (AmountFrom != null)
                {
                    yield return "At least £" + AmountFrom;
                }
                if (AmountTo != null)
                {
                    yield return "At most £" + AmountTo;
                }
                if (Person != null)
                {
                    yield return "Paid by " + Person;
                }
            }
        }
    }
}