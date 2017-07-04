using FinancesSharp.Binders;
using FinancesSharp.Models;
using FinancesSharp.Reports.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FinancesSharp.Reports
{
    [ModelBinder(typeof(UnusualSpendingByCategoryBinder))]
    public class UnusualSpendingByCategory : AverageMonthlySpendingByCategory
    {
        public IEnumerable<CategoryComparison> HigherSpendingData { get; private set; }
        public IEnumerable<CategoryComparison> LowerSpendingData { get; private set; }
        public MonthAndYear SelectedMonth { get; set; }

        public UnusualSpendingByCategory()
        {
            SelectedMonth = MonthAndYear.Current.AddMonths(-1);
        }

        public override string FriendlyName
        {
            get
            {
                return "Where have we spent more than usual?";
            }
        }

        protected override IEnumerable<CategoryAmount> calculate(FinanceDb db, IQueryable<Transaction> transactions)
        {
            var selectedMonthData = calculateSelectedMonthData(db);
            var averageData = base.calculate(db, transactions);

            var categories = averageData
                .Select(x => x.Category)
                .Intersect(selectedMonthData.Select(x => x.Category))
                .ToList();

            var data = categories.Select(category => new CategoryComparison(
                category,
                averageData.Single(x => x.Category == category).Amount,
                selectedMonthData.Single(x => x.Category == category).Amount)
            ).ToList();

            HigherSpendingData = data
                .Where(x => x.Difference > 0)
                .OrderByDescending(x => x.Difference)
                .ToList();
            LowerSpendingData = data
                .Where(x => x.Difference < 0)
                .OrderBy(x => x.Difference)
                .ToList();

            return averageData;
        }

        private IEnumerable<CategoryAmount> calculateSelectedMonthData(FinanceDb db)
        {
            var search = new Search()
            {
                DateFrom = SelectedMonth.ToDateTime(MonthAndYear.PositionInMonth.Start),
                DateTo = SelectedMonth.ToDateTime(MonthAndYear.PositionInMonth.End),
            };
            var selectedMonthTotals = db.Transactions
                .Where(search.IsMatch())
                .GroupBy(x => x.Category)
                .ToList()
                .Select(group => new CategoryMonthAmount(group.Key, SelectedMonth, group.Sum(x => x.Amount)));
            var selectedMonthData = new AverageOverMonthsCategoryRoller(Category, SelectedMonth, SelectedMonth)
                .RollUpByCategoriesOfInterest(db, selectedMonthTotals)
                .ToList();
            return filter(selectedMonthData);
        }
    }
}