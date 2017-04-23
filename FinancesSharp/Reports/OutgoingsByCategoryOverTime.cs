using FinancesSharp.Binders;
using FinancesSharp.Models;
using FinancesSharp.Reports.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FinancesSharp.Reports
{
    [ModelBinder(typeof(OutgoingsByCategoryOverTimeBinder))]
    public class OutgoingsByCategoryOverTime : ByCategoryReport<CategoryMonthAmount>
    {
        public override string FriendlyName
        {
            get { return "How is our spending changing?"; }
        }

        public IEnumerable<TimeSeries<Category>> Data
        {
            get
            {
                var categories = data.Select(x => x.Category).Distinct();
                foreach (var category in categories.OrderBy(x => x.Name))
                {
                    var datapoints = data.Where(x => x.Category == category).ToList();
                    if (datapoints.Any(x => x.Amount != 0))
                    {
                        yield return new TimeSeries<Category>(category.Name, toDatapoints(datapoints), drillDownIdFor(category));
                    }
                }
            }
        }
        private IEnumerable<TimeSeriesDatapoint> toDatapoints(IEnumerable<CategoryMonthAmount> data)
        {
            foreach (var monthTotal in data.OrderBy(x => x.Month))
            {
                yield return new TimeSeriesDatapoint()
                {
                    Date = monthTotal.Month.ToDateTime(),
                    Value = monthTotal.Amount,
                    HumanReadableValue = monthTotal.Amount.ToString("C0"),
                };
            }
        }

        protected override IEnumerable<CategoryMonthAmount> calculate(FinanceDb db, IQueryable<Transaction> transactions)
        {
            IEnumerable<CategoryMonthAmount> data;
            data = transactions
                .GroupBy(x => new { x.Category, x.Date.Year, x.Date.Month })
                .ToList()
                .Select(group => new CategoryMonthAmount(group.Key.Category, new MonthAndYear(group.Key.Year, group.Key.Month), group.Sum(x => x.Amount)))
                .ToList();
            data = new PerMonthCategoryRoller(Category).RollUpEachMonthByCategoriesOfInterest(db, data);
            return data.ToList();
        }

        public class PerMonthCategoryRoller : CategoryRoller<CategoryMonthAmount>
        {
            public PerMonthCategoryRoller(Category topLevelCategory)
                : base(topLevelCategory) { }

            public IEnumerable<CategoryMonthAmount> RollUpEachMonthByCategoriesOfInterest(FinanceDb db, IEnumerable<CategoryMonthAmount> totals)
            {
                foreach (var month in totals.Select(x => x.Month).Distinct())
                {
                    var subset = totals.Where(x => x.Month == month);
                    foreach (CategoryAmount categoryAmount in RollUpByCategoriesOfInterest(db, subset))
                    {
                        yield return new CategoryMonthAmount(categoryAmount.Category, month, categoryAmount.Amount);
                    }
                }
            }

            protected override decimal rollUpCategory(IEnumerable<CategoryMonthAmount> categoryTotals)
            {
                return Math.Round(categoryTotals.Sum(x => x.Amount), 0);
            }
        }
    }
}