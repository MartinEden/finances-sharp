using FinancesSharp.Models;
using FinancesSharp.Reports.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancesSharp.Reports
{
    public interface IByCategoryReport
    {
        Category Category { get; set; }
        DateTime? DateFrom { get; set; }
        DateTime? DateTo { get; set; }
        string ParentCategoryId { get; }
    }

    public abstract class ByCategoryReport<TDataItem> : Report, IByCategoryReport
    {
        public Category Category { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        protected IEnumerable<TDataItem> data { get; private set; }

        public ByCategoryReport()
        {
            var previousMonth = MonthAndYear.Current.AddMonths(-1);
            DateFrom = previousMonth.ToDateTime(MonthAndYear.PositionInMonth.Start);
            DateTo = previousMonth.ToDateTime(MonthAndYear.PositionInMonth.End);
        }

        public override void Run(FinanceDb db)
        {
            var transactions = retrieve(db);
            data = calculate(db, transactions);
        }
        public string ParentCategoryId
        {
            get
            {
                if (Category == null || Category.Parent == null)
                {
                    return "";
                }
                else
                {
                    return Category.Parent.Id.ToString();
                }
            }
        }

        private IQueryable<Transaction> retrieve(FinanceDb db)
        {
            var search = new Search()
            {
                Category = Category,
                DateFrom = DateFrom,
                DateTo = DateTo
            };
            return db.Transactions.Where(search.IsMatch());
        }
        protected abstract IEnumerable<TDataItem> calculate(FinanceDb db, IQueryable<Transaction> transactions);

        protected int? drillDownIdFor(Category category)
        {
            if (category == Category)
            {
                return null;
            }
            if (category.DirectChildren.Any())
            {
                return category.Id;
            }
            else
            {
                return null;
            }
        }
    }
}