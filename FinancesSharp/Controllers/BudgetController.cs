using System;
using System.ComponentModel.DataAnnotations;
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
            var budget = GetBudget(id);
            var viewModel = new BudgetViewModel(budget, Db);
            return Json(viewModel.Flatten(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateMiscBudgetAmount(int id, UpdateMiscellaneousBudgetViewModel viewModel)
        {
            var budget = GetBudget(id);
            if (TryValidateModel(viewModel))
            {
                budget.MiscellaneousBudget = viewModel.MiscellaneousBudget;
                Db.SaveChanges();
                return Json("OK");
            }
            return ValidationErrorsAsJson();
        }

        [HttpPost]
        public ActionResult CreateBudgetItem(int id, NewBudgetItem newItem)
        {
            var budget = GetBudget(id);
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
            return ValidationErrorsAsJson();
        }

        [HttpPost]
        public ActionResult UpdateBudgetItem(int id, UpdateBudgetItem viewModel)
        {
            var budget = GetBudget(id);
            if (TryValidateModel(viewModel))
            {
                var item = budget.Items.SingleOrDefault(x => x.Id == viewModel.ItemId);
                if (item != null)
                {
                    item.Name = viewModel.Name;
                    item.BudgetedAmount = viewModel.BudgetedAmount;
                    Db.SaveChanges();
                    return Json("OK");
                }
                else
                {
                    ModelState.AddModelError("ItemId", $"Unknown item '{viewModel.ItemId}'");
                }
            }
            return ValidationErrorsAsJson();
        }

        [HttpPost]
        public ActionResult DeleteBudgetItem(int id, int itemId)
        {
            var budget = GetBudget(id);
            var item = budget.Items.SingleOrDefault(x => x.Id == itemId);
            if (item != null)
            {
                budget.Items.Remove(item);
                Db.SaveChanges();
                return Json("OK");
            }
            Response.StatusCode = 400;
            return Json($"Unknown budget item {itemId}");
        }
        
        [HttpPost]
        public ActionResult RestoreBudgetItem(int id, int itemId)
        {
            var budget = GetBudget(id);
            var item = Db.BudgetItems.SingleOrDefault(x => x.Id == itemId);
            if (item != null)
            {
                budget.Items.Add(item);
                Db.SaveChanges();
                return Json("OK");
            }
            Response.StatusCode = 400;
            return Json($"Unknown budget item {itemId}");
        }

        [HttpPost]
        public ActionResult AddCategory(int id, CategoryToBudgetItemViewModel viewModel)
        {
            return WithCategoryAndBudgetItem(id, viewModel, (budget, item, category) =>
            {
                foreach (var cat in category.Children)
                {
                    item.Categories.Add(cat);
                }
                Db.SaveChanges();
                return Json("OK");
            });
        }

        [HttpPost]
        public ActionResult RemoveCategory(int id, CategoryToBudgetItemViewModel viewModel)
        {
            return WithCategoryAndBudgetItem(id, viewModel, (budget, item, category) =>
            {
                item.Categories.Remove(category);
                Db.SaveChanges();
                return Json("OK");
            });
        }

        private ActionResult WithCategoryAndBudgetItem(int budgetId, 
            CategoryToBudgetItemViewModel viewModel, 
            Func<Budget, BudgetItem, Category, ActionResult> work)
        {
            var budget = GetBudget(budgetId);
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
                    return work(budget, item, category);
                }
            }
            return ValidationErrorsAsJson();
        }

        private Budget GetBudget(int id)
        {
            return Db.Budgets.Single(x => x.Id == id);
        }

        private ActionResult ValidationErrorsAsJson()
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