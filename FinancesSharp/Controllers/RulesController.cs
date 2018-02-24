using FinancesSharp.ViewModels;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace FinancesSharp.Controllers
{
    public class RulesController : FinancesControllerBase
    {
        public ActionResult Index()
        {
            return View(new RulesViewModel(Db));
        }

        public ActionResult Stale()
        {
            var vm = new StaleRulesViewModel(Db.ActiveRules.ToList(), Db.Transactions);
            return View(vm);
        }

        public ActionResult Search(int id)
        {
            var rule = Db.ActiveRules.Single(r => r.Id == id);
            return RedirectToAction("Index", "Home", new IndexViewModel(new SearchForm(rule.Criteria)).ToRouteValues());
        }

        [HttpPost]
        public ActionResult Deactivate(int id)
        {
            var rule = Db.ActiveRules.Single(r => r.Id == id);
            rule.Active = false;
            Db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}