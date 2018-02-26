using System.Linq;
using System.Web.Mvc;
using FinancesSharp.Models;
using FinancesSharp.ViewModels;

namespace FinancesSharp.Controllers
{
    public class BudgetController : FinancesControllerBase
    {
        public ActionResult Index()
        {
            var budget = Db.GetOrCreateDefaultBudget();
            return View(budget);
        }
        
        public ActionResult Get(int id)
        {
            var budget = Db.Budgets.Single(x => x.Id == id);
            return Json(budget, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateBudgetItem(int id, NewBudgetItem newItem)
        {
            var budget = Db.Budgets.Single(x => x.Id == id);
            if (TryValidateModel(newItem))
            {
                budget.Items.Add(new BudgetItem
                {
                    Name = newItem.Name,
                    BudgetedAmount = newItem.BudgetedAmount
                });
                Db.SaveChanges();
                return Json("OK");
            }
            else
            {
                Response.StatusCode = 400;
                return Json(ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .Distinct()
                );
            }
        }
    }
}