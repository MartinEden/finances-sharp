using FinancesSharp.Csv;
using FinancesSharp.Helpers;
using FinancesSharp.Models;
using FinancesSharp.ViewModels;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinancesSharp.Controllers
{
    public class HomeController : FinancesControllerBase
    {
        public ActionResult Index(IndexViewModel vm)
        {
            vm.Initialize(Db);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ChangeName(int id, string name)
        {
            var trx = Db.Transactions.Single(t => t.Id == id);
            trx.Name = name;
            if (TryValidateModel(trx))
            {
                Db.SaveChanges();
            }
            return new ContentResult();
        }

        public ActionResult UploadTransactions(HttpPostedFileBase file)
        {
            var result = new UploadTransactionsResult(Db);
            if (file != null)
            {
                var p = new StatementParser(file.InputStream, Db);
                if (!p.Errors.Any())
                {
                    Db.Backup();

                    var autoCategoriser = new AutoCategoriser(Db);
                    autoCategoriser.Process(p.Transactions, Db);
                    result.Rules = autoCategoriser.Results;
                    result.Transactions = p.Transactions;
                }
                result.Errors = p.Errors;
            }
            return View(result);
        }

        [HttpPost]
        public ActionResult Categorise(IndexViewModel vm)
        {
            vm.Initialize(Db);
            if (TryValidateModel(vm.Categorisation))
            {
                foreach (var trx in vm.AllTransactions)
                {
                    trx.Category = vm.Categorisation.Category;
                }
                Db.SaveChanges();
                if (vm.Categorisation.CreateRule)
                {
                    var rule = Db.CategorisationRules.Add(new CategorisationRule(vm.Search.Search, vm.Categorisation.Category));
                    Db.SaveChanges();
                }
                return RedirectToAction("Index", new IndexViewModel(vm.Search).ToRouteValues());
            }
            else
            {
                vm.Categorisation.Show();
                return View("Index", vm);
            }
        }
    }
}
