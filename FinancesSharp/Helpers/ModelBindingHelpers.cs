using FinancesSharp.Controllers;
using FinancesSharp.Models;
using System.Web.Mvc;

namespace FinancesSharp.Helpers
{
    public static class ModelBindingHelpers
    {
        public static FinanceDb Db(this ControllerContext controllerContext)
        {
            return ((IFinancesController)controllerContext.Controller).Db;
        }
    }
}