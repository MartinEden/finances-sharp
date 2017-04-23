using FinancesSharp.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FinancesSharp.Helpers
{
    public class AutoCategoriser
    {
        private IDictionary<CategorisationRule, List<Transaction>> rules;

        public AutoCategoriser(FinanceDb db)
        {
            rules = db.ActiveRules.ToDictionary(r => r, r => new List<Transaction>());
        }

        public void Process(Transaction trx)
        {
            foreach (var rule in rules.Keys)
            {                
                if (rule.Matches(trx))
                {
                    rule.ApplyTo(trx);
                    rules[rule].Add(trx);
                    break;
                }
            }
        }

        public IDictionary<CategorisationRule, List<Transaction>> Results
        {
            get { return rules; }
        }

        public void Process(IEnumerable<Transaction> transactions, FinanceDb db)
        {
            foreach (var trx in transactions)
            {
                Process(trx);
                db.Transactions.Add(trx);
            }
            db.SaveChanges();
        }
    }
}