using System.Collections.Generic;
using FinancesSharp.Models;
using System.Linq;
using System;

namespace FinancesSharp.ViewModels
{
    public class UploadTransactionsResult
    {
        public IEnumerable<string> Errors { get; set; }
        public IDictionary<CategorisationRule, List<Transaction>> Rules { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
        public DateTime? MostRecentTransactionDate { get; set; }

        public UploadTransactionsResult()
        {
            Errors = new List<string>();
            Transactions = new List<Transaction>();
        }
        public UploadTransactionsResult(FinanceDb db)
            : this()
        {
            var mostRecentTransaction = db.Transactions.OrderByDescending(x => x.Date).FirstOrDefault();
            if (mostRecentTransaction != null)
            {
                MostRecentTransactionDate = mostRecentTransaction.Date;
            }
        }
        
        public int NumberAutocategorised
        {
            get { return Rules.SelectMany(x => x.Value).Count(); }
        }
        public DateTime Earliest
        {
            get { return Transactions.Min(x => x.Date); }
        }
        public DateTime Latest
        {
            get { return Transactions.Max(x => x.Date); }
        }
    }
}