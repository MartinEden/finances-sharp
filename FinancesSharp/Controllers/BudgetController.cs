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
            var budget = getBudget(id);
            return Json(budget.Flatten(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateBudgetItem(int id, NewBudgetItem newItem)
        {
            var budget = getBudget(id);
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
            return validationErrorsAsJson();
        }

        [HttpPost]
        public ActionResult AddCategory(int id, AddCategoryToBudgetItem viewModel)
        {
            var budget = getBudget(id);
            if (TryValidateModel(viewModel))
            {
                var item = budget.Items.SingleOrDefault(x => x.Id == viewModel.ItemId);
                if (item == null)
                {
                    ModelState.AddModelError("ItemId", $"Budget item '{viewModel.ItemId}' does not belong to this budget");
                }
                var category = Db.Categories.SingleOrDefault(x => x.Id == viewModel.CategoryId);
                if (category == null)
                {
                    ModelState.AddModelError("CategoryId", $"Unknown category '{viewModel.CategoryId}'.");
                }
                if (ModelState.IsValid)
                {
                    foreach (var cat in category.Children)
                    {
                        item.Categories.Add(cat);
                    }
                    Db.SaveChanges();
                    return Json("OK");
                }
            }
            return validationErrorsAsJson();
        }

        private Budget getBudget(int id)
        {
            return Db.Budgets.Single(x => x.Id == id);
        }

        private ActionResult validationErrorsAsJson()
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