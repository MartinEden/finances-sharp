using FinancesSharp.Models;

namespace FinancesSharp.Reports.Helpers
{
    public struct CategoryMonthAmount : IHasCategory
    {
        public Category Category { get; private set; }
        public readonly MonthAndYear Month;
        public readonly decimal Amount;

        public CategoryMonthAmount(Category category, MonthAndYear month, decimal amount)
        {
            Category = category;
            Month = month;
            Amount = amount;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Category, Month);
        }
    }
}