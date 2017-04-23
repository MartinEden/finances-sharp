using FinancesSharp.Binders;
using FinancesSharp.Models;
using FinancesSharp.Reports.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FinancesSharp.Reports
{
    [ModelBinder(typeof(AverageMonthlySpendingByCategoryBinder))]
    public class AverageMonthlySpendingByCategory : ByCategoryReport<CategoryAmount>
    {
        public MonthAndYear StartMonth
        {
            get { return new MonthAndYear(DateFrom.Value); }
            set
            {
                DateFrom = value.ToDateTime(MonthAndYear.PositionInMonth.Start);
            }
        }
        public MonthAndYear EndMonth
        {
            get { return new MonthAndYear(DateTo.Value); }
            set
            {
                DateTo = value.ToDateTime(MonthAndYear.PositionInMonth.End);
            }
        }

        public override string FriendlyName
        {
            get { return "How much do we normally spend?"; }
        }

        public IEnumerable<CategoryAmount> Data
        {
            get { return data; }
        }

        protected override IEnumerable<CategoryAmount> calculate(FinanceDb db, IQueryable<Transaction> transactions)
        {
            var monthAndCategoryTotals = transactions
                .GroupBy(x => new { x.Category, x.Date.Year, x.Date.Month })
                .ToList()
                .Select(group => new CategoryMonthAmount(group.Key.Category, new MonthAndYear(group.Key.Year, group.Key.Month), group.Sum(x => x.Amount)))
                .ToList();
            var data = new AverageOverMonthsCategoryRoller(Category, StartMonth, EndMonth).RollUpByCategoriesOfInterest(db, monthAndCategoryTotals);
            data = filter(data);
            return data
                .OrderBy(x => x.Category.Name)
                .ToList();
        }

        protected virtual IEnumerable<CategoryAmount> filter(IEnumerable<CategoryAmount> data)
        {
            return data
                .Where(x => x.Amount < 0)
                .Select(x => new CategoryAmount(x.Category, Math.Abs(x.Amount)));
        }

        protected class AverageOverMonthsCategoryRoller : CategoryRoller<CategoryMonthAmount>
        {
            private IEnumerable<MonthAndYear> monthsOfInterest;
            private MonthAndYear startMonth;
            private MonthAndYear endMonth;

            public AverageOverMonthsCategoryRoller(Category topLevelCategory, MonthAndYear startMonth, MonthAndYear endMonth)
                : base(topLevelCategory)
            {
                this.startMonth = startMonth;
                this.endMonth = endMonth;
            }

            protected override void initialize(IEnumerable<CategoryMonthAmount> totals)
            {
                base.initialize(totals);
                monthsOfInterest = monthRange(startMonth, endMonth).ToList();

                var badMonths = totals.Select(x => x.Month).Except(monthsOfInterest);
                if (badMonths.Any())
                {
                    throw new InvalidOperationException(string.Format("Data contained these months [{0}] which were outside the window from '{1}' to '{2}'",
                        string.Join(", ", badMonths), startMonth, endMonth));
                }
            }

            private IEnumerable<MonthAndYear> monthRange(MonthAndYear start, MonthAndYear end)
            {
                var month = start;
                while (month <= end)
                {
                    yield return month;
                    month = month.AddMonths(1);
                }
            }

            protected override decimal rollUpCategory(IEnumerable<CategoryMonthAmount> categoryTotals)
            {
                var monthTotals = monthsOfInterest.Select(month => categoryTotals.Where(t => t.Month == month).Sum(t => t.Amount));
                return monthTotals.Sum() / monthsOfInterest.Count();
            }
        }
    }
}