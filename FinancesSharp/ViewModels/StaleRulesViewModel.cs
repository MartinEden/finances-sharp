using FinancesSharp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FinancesSharp.ViewModels
{
    public class StaleRulesViewModel
    {
        public IEnumerable<CategorisationRuleWithStatus> Rules { get; private set; }

        public StaleRulesViewModel(IEnumerable<CategorisationRule> all, DbSet<Transaction> transactions)
        {
            var cutoff = DateTime.Today - TimeSpan.FromDays(180);// ~6 months
            var rules = new List<CategorisationRuleWithStatus>();
            foreach (var rule in all)
            {
                var matches = transactions.Where(rule.Criteria.IsMatch());
                var mostRecentMatch = matches.OrderByDescending(x => x.Date).FirstOrDefault();
                if (mostRecentMatch != null)
                {
                    if (mostRecentMatch.Date < cutoff)
                    {
                        rules.Add(new CategorisationRuleWithStatus(rule, mostRecentMatch.Date));
                    }
                }
                else
                {
                    rules.Add(new CategorisationRuleWithStatus(rule, null));
                }
            }
            Rules = rules.OrderBy(x => x.MostRecentMatch).ToList();
        }
    }

    public class CategorisationRuleWithStatus
    {
        public readonly CategorisationRule Rule;
        public readonly DateTime? MostRecentMatch;
        
        public CategorisationRuleWithStatus(CategorisationRule rule, DateTime? mostRecentMatch)
        {
            Rule = rule;
            MostRecentMatch = mostRecentMatch;
        }
    }
}