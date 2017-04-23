using FinancesSharp.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace FinancesSharp.ViewModels
{
    public class TransactionSortMode
    {
        public SortColumn Column { get; set; }
        public SortOrder Order { get; set; }

        public IQueryable<Transaction> Sort(IQueryable<Transaction> transactions)
        {
            switch (Column)
            {
                case SortColumn.Date:
                    switch (Order)
                    {
                        case SortOrder.Ascending:
                            return transactions.OrderBy(x => x.Date);
                        case SortOrder.Descending:
                            return transactions.OrderByDescending(x => x.Date);
                    }
                    break;
                case SortColumn.Amount:
                    switch (Order)
                    {
                        case SortOrder.Ascending:
                            return transactions.OrderBy(x => x.Amount);
                        case SortOrder.Descending:
                            return transactions.OrderByDescending(x => x.Amount);
                    }
                    break;
            }
            throw new NotSupportedException(string.Format("Unsupported transaction sort mode: '{0}'", this));
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Column, Order);
        }

        public enum SortColumn
        {
            Date = 0,
            Amount = 1,
        }
        public enum SortOrder
        {
            Descending = 0,
            Ascending = 1,
        }
    }

    public static class TransactionSortModeHtmlHelperExtensions
    {
        public static MvcHtmlString DisplaySortArrows<TModel>(this HtmlHelper<TModel> html, TransactionSortMode.SortColumn column)
        {
            return html.Partial("TransactionSortArrows", column);
        }
    }
}