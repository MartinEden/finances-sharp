using FinancesSharp.Models;
using System.Web.Mvc;

namespace FinancesSharp.Controllers
{
    public abstract class FinancesControllerBase : Controller, IFinancesController
    {
        public FinanceDb Db { get; private set; }

        public FinancesControllerBase()
        {
            Db = new FinanceDb();
        }
    }
}