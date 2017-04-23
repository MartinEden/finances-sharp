using FinancesSharp.Reports;
using System.Web.Mvc;

namespace FinancesSharp.Binders
{
    public class ByCategoryBinder<TModel> : Binder<TModel>
        where TModel : IByCategoryReport
    {
        protected override void bindProperties(ControllerContext c, ModelBindingContext b)
        {
            bindNavigationProperty(x => x.Category, c, b);
            bindDate(x => x.DateFrom, c, b);
            bindDate(x => x.DateTo, c, b);
        }
    }

    public class OutgoingsByCategoryBinder : ByCategoryBinder<OutgoingsByCategory> { }
    public class IncomeByCategoryBinder : ByCategoryBinder<IncomeByCategory> { }
    public class AverageMonthlySpendingByCategoryBinder : ByCategoryBinder<AverageMonthlySpendingByCategory> { }
    public class UnusualSpendingByCategoryBinder : ByCategoryBinder<UnusualSpendingByCategory>
    {
        protected override void bindProperties(ControllerContext c, ModelBindingContext b)
        {
            // Note that we do not bind the underlying DateFrom and DateTo properties - we don't want them for this report
            bindNavigationProperty(x => x.Category, c, b);
            bindMonthAndYear(x => x.SelectedMonth, c, b);
        }
    }
    public class OutgoingsByCategoryOverTimeBinder : ByCategoryBinder<OutgoingsByCategoryOverTime> { }
}