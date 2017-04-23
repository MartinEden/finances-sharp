using FinancesSharp.Models;

namespace FinancesSharp.Reports.Helpers
{
    public struct CategoryAmount : IHasCategory
    {
        public Category Category { get; private set; }
        public readonly decimal Amount;

        public CategoryAmount(Category category, decimal amount)
        {
            Category = category;
            Amount = amount;
        }
    }
}