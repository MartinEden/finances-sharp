using FinancesSharp.Helpers;
using FinancesSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Routing;

namespace FinancesSharp.ViewModels
{
    public class IndexViewModel : IConvertibleToRouteValues
    {
        public IEnumerable<Transaction> Transactions { get; set; }
        public IQueryable<Transaction> AllTransactions { get; private set; }
        public SearchForm Search { get; set; }
        public TransactionSortMode Sort { get; set; }

        public CategorisationForm Categorisation { get; set; }

        public int NextLimit { get; set; }
        public int CurrentLimit { get; private set; }
        public int Total { get; private set; }
        public const int DefaultLimit = 100;

        public IndexViewModel() 
            : this(new SearchForm()) { }
        public IndexViewModel(SearchForm search)
        {
            Search = search;
            NextLimit = DefaultLimit;
            Sort = new TransactionSortMode();
            Categorisation = new CategorisationForm();
        }

        public void Initialize(FinanceDb db)
        {
            Search.Initialize(db);
            var transactions = db.Transactions
                .Include(x => x.Category)
                .Include(x => x.Card.Person);
            AllTransactions = Sort.Sort(Search.Filter(transactions));
            Total = AllTransactions.Count();
            Transactions = AllTransactions.Take(NextLimit).AsEnumerable();
            CurrentLimit = NextLimit;
            NextLimit = DefaultLimit;
        }

        public bool MoreToShow
        {
            get
            {
                return Total > CurrentLimit;
            }
        }

        public decimal TotalChangeInBalance
        {
            get
            {
                if (Transactions != null)
                {
                    return Transactions.Sum(x => x.Amount);
                }
                return 0;
            }
        }

        public RouteValueDictionary ToRouteValues()
        {
            var dict = new RouteValueDictionary();
            dict.AddSubDictionary(this, x => x.Search);
            return dict;
        }
    }
}