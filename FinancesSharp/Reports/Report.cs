using FinancesSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FinancesSharp.Reports
{
    public abstract class Report
    {
        public abstract string FriendlyName { get; }
        public string InternalName
        {
            get
            {
                return this.GetType().Name;
            }
        }
        public virtual string FormalName
        {
            get
            {
                var wordRegex = new Regex(@"([A-Z][a-z]*)");
                var matches = wordRegex.Matches(InternalName);
                var words = matches.Cast<Match>().Select(m => m.Groups[1].Value);
                var spaced = string.Join(" ", words).ToLower();
                return spaced.Substring(0, 1).ToUpper() + spaced.Substring(1);
            }
        }
        public virtual string ViewName
        {
            get { return FormalName; }
        }

        public abstract void Run(FinanceDb db);

        public static readonly IEnumerable<Report> All = findAllReports();

        private static IEnumerable<Report> findAllReports()
        {
            return Assembly.GetAssembly(typeof(Report))
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => typeof(Report).IsAssignableFrom(t))
                .Select(t => Activator.CreateInstance(t))
                .Cast<Report>()
                .ToList();
        }
    }
}