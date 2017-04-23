using FinancesSharp.Models;
using FinancesSharp.Reports.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancesSharp.Reports
{
    public abstract class TotalAmountByCategory : ByCategoryReport<CategoryAmount>
    {
        public override string ViewName
        {
            get { return "TotalAmountByCategory"; }
        }

        public IEnumerable<BarChartDatapoint> Data
        {
            get
            {
                var grandTotal = data.Sum(x => x.Amount);
                foreach (var total in data)
                {
                    string mainLabel = total.Category == Category ? "Miscellaneous" : total.Category.Name;
                    yield return new BarChartDatapoint(mainLabel, 
                        percentageFor(total, grandTotal).ToString(), 
                        total.Amount,
                        total.Amount.ToString("C0"),
                        drillDownIdFor(total.Category));
                }
            }
        }

        private object percentageFor(CategoryAmount total, decimal grandTotal)
        {
            var fraction = total.Amount / grandTotal;
            if (fraction < 0.01M)
            {
                return "<1%";
            }
            return fraction.ToString("P0");
        }

        public decimal Maximum
        {
            get
            {
                if (data.Any())
                {
                    return data.Max(x => x.Amount);
                }
                return 0;
            }
        }      

        protected override IEnumerable<CategoryAmount> calculate(FinanceDb db, IQueryable<Transaction> transactions)
        {
            var totals = transactions
                .GroupBy(trx => trx.Category)
                .ToList()
                .Select(group => new CategoryAmount(group.Key, group.Sum(g => g.Amount)))
                .ToList();
            var data = new TotalAmountCategoryRoller(Category).RollUpByCategoriesOfInterest(db, totals);
            data = filter(data);
            return data
                .OrderByDescending(x => x.Amount)
                .ToList();
        }

        protected abstract IEnumerable<CategoryAmount> filter(IEnumerable<CategoryAmount> data);

        public class TotalAmountCategoryRoller : CategoryRoller<CategoryAmount>
        {
            public TotalAmountCategoryRoller(Category topLevelCategory)
                : base(topLevelCategory) { }

            protected override decimal rollUpCategory(IEnumerable<CategoryAmount> categoryTotals)
            {
                return Math.Round(categoryTotals.Sum(x => x.Amount), 0);
            }
        }
    }
}