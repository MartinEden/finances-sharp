using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FinancesSharp.Binders;
using FinancesSharp.Models;
using FinancesSharp.Reports.Helpers;

namespace FinancesSharp.Reports
{
    [ModelBinder(typeof(BudgetReportBinder))]
    public class BudgetReport : Report
    {
        public MonthAndYear Month { get; set; }
        public Budget Budget { get; set; }
        public IEnumerable<BudgetReportItem> Data { get; private set; }
        public decimal MiscellaneousSpending { get; private set; }
        
        public BudgetReport()
        {
            Month = MonthAndYear.Current.AddMonths(-1);
        }

        public override string FriendlyName
        {
            get { return "Is our spending within budget?"; }
        }

        public decimal TotalSpending
        {
            get
            {
                if (Data != null)
                {
                    return Data.Sum(x => x.TotalSpending) + MiscellaneousSpending;
                }
                else
                {
                    return 0;
                }
            }
        }

        public decimal Surplus
        {
            get { return Budget.TotalBudget - TotalSpending; }
        }

        public decimal MiscellaneousSurplus
        {
            get { return Budget.MiscellaneousBudget - MiscellaneousSpending; }
        }

        public override void Run(FinanceDb db)
        {
            Budget = db.GetOrCreateDefaultBudget();
            var transactions = retrieve(db).GroupBy(x => x.Category)
                .ToList();
            Data = Budget.Items.Select(item => new BudgetReportItem(
                item, 
                getTotalForCategories(transactions, item.Categories)
            )).ToList();
            
            MiscellaneousSpending = getTotalForCategories(transactions, Budget.SpareCategories(db));
        }

        private IQueryable<Transaction> retrieve(FinanceDb db)
        {
            var search = new Search
            {
                DateFrom = Month.ToDateTime(MonthAndYear.PositionInMonth.Start),
                DateTo = Month.ToDateTime(MonthAndYear.PositionInMonth.End)
            };
            return db.Transactions.Where(search.IsMatch());
        }

        private decimal getTotalForCategories(
            IEnumerable<IGrouping<Category, Transaction>> transactions,
            IEnumerable<Category> categories
        )
        {
            // We invert the result here, because transactions are stored where +ve is income and -ve is spending,
            // but this report uses +ve to mean spending
            return -transactions.Where(x => categories.Contains(x.Key)).SelectMany(x => x).Sum(x => x.Amount);
        }
    }

    public class BudgetReportItem
    {
        public readonly BudgetItem Item;
        public readonly decimal TotalSpending;

        public BudgetReportItem(BudgetItem item, decimal totalSpending)
        {
            Item = item;
            TotalSpending = totalSpending;
        }

        public decimal Surplus
        {
            get { return Item.BudgetedAmount - TotalSpending; }
        }
    }
}