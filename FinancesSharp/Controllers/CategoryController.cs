using FinancesSharp.Models;
using FinancesSharp.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace FinancesSharp.Controllers
{
    public class CategoryController : FinancesControllerBase
    {
        public ActionResult All()
        {
            return Json(CategoryTree.Build(Db), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Change(int id, int categoryId)
        {
            var trx = Db.Transactions.Single(t => t.Id == id);
            var category = Db.Categories.Single(c => c.Id == categoryId);
            trx.Category = category;
            Db.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public ActionResult Create(string name, int? parentId)
        {
            Category parent = null;
            if (parentId.HasValue)
            {
                parent = Db.Categories.Single(c => c.Id == parentId);
            }
            var category = Db.Categories.Add(new Category(name, parent));
            if (TryValidateModel(category))
            {
                Db.SaveChanges();
                return Json(true);
            }
            return Json(false);
        }
    }
}