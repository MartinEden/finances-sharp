using FinancesSharp.Binders;
using FinancesSharp.Reports.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FinancesSharp.Reports
{
    [ModelBinder(typeof(OutgoingsByCategoryBinder))]
    public class OutgoingsByCategory : TotalAmountByCategory
    {        
        public override string FriendlyName
        {
            get { return "What have we spent money on?"; }
        }

        protected override IEnumerable<CategoryAmount> filter(IEnumerable<CategoryAmount> data)
        {
            return data
                .Where(x => x.Amount < 0)
                .Select(x => new CategoryAmount(x.Category, Math.Abs(x.Amount)));
        }
    }
}