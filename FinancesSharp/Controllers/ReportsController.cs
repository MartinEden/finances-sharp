using FinancesSharp.Reports;
using System.Web.Mvc;

namespace FinancesSharp.Controllers
{
    public class ReportsController : FinancesControllerBase
    {
        public ActionResult Index()
        {
            return View(Report.All);
        }

        public ActionResult OutgoingsByCategory([Bind(Prefix = "")] OutgoingsByCategory report)
        {
            return viewReport(report);
        }
        public ActionResult IncomeByCategory([Bind(Prefix = "")] IncomeByCategory report)
        {
            return viewReport(report);
        }
        public ActionResult AverageMonthlySpendingByCategory([Bind(Prefix = "")] AverageMonthlySpendingByCategory report)
        {
            return viewReport(report);
        }
        public ActionResult UnusualSpendingByCategory([Bind(Prefix = "")] UnusualSpendingByCategory report)
        {
            return viewReport(report);
        }
        public ActionResult OutgoingsByCategoryOverTime([Bind(Prefix = "")] OutgoingsByCategoryOverTime report)
        {
            return viewReport(report);
        }

        private ActionResult viewReport(Report report)
        {
            report.Run(Db);
            return View("ViewReport", report);
        }
    }
}