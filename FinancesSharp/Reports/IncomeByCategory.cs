using FinancesSharp.Binders;
using FinancesSharp.Reports.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FinancesSharp.Reports
{
    [ModelBinder(typeof(IncomeByCategoryBinder))]
    public class IncomeByCategory : TotalAmountByCategory
    {
        public override string FriendlyName
        {
            get { return "Where has the money come from?"; }
        }

        protected override IEnumerable<CategoryAmount> filter(IEnumerable<CategoryAmount> data)
        {
            return data.Where(x => x.Amount > 0);
        }
    }
}