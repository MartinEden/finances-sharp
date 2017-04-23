using FinancesSharp.Models;
using System;

namespace FinancesSharp.Reports.Helpers
{
    public class CategoryComparison
    {
        public readonly Category Category;
        public readonly decimal AverageAmount;
        public readonly decimal SelectedAmount;

        public CategoryComparison(Category category, decimal averageAmount, decimal selectedAmount)
        {
            Category = category;
            AverageAmount = averageAmount;
            SelectedAmount = selectedAmount;
        }

        public decimal Ratio
        {
            get { return SelectedAmount / AverageAmount; }
        }

        public decimal Difference
        {
            get
            {
                return SelectedAmount - AverageAmount;
            }
        }
        public decimal PercentageDifference
        {
            get
            {
                if (AverageAmount != 0)
                {
                    return 100 * (Ratio - 1);
                }
                return 0;
            }
        }
        public string CssColor
        {
            get
            {
                var excess = Math.Max(Math.Min(Ratio - 1, 1), 0);
                byte red = (byte)(excess * 255);
                return string.Format("rgb({0}, 0, 0)", red);
            }
        }
    }
}