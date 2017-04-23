using FinancesSharp.ViewModels;
using System.Web.Mvc;
using System.Linq;

namespace FinancesSharp.Controllers
{
    public class AdminController : FinancesControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Backup()
        {
            if (Request.HttpMethod == "POST")
            {
                Db.Backup();
                return View(true);
            }
            return View(false);
        }

        public ActionResult Categories(string message = "")
        {
            return View("Categories", new EditCategoryViewModel(message));
        }
        [HttpPost]
        public ActionResult ChangeCategoryName(int id, string name)
        {
            var category = Db.Categories.Find(id);
            string oldName = category.Name;
            category.Name = name;
            Db.SaveChanges();
            return Categories(string.Format("Renamed '{0}' to '{1}'.", oldName, name));
        }
        [HttpPost]
        public ActionResult MoveCategory([Bind(Prefix = "")] EditCategoryViewModel vm)
        {
            var category = vm.Category;
            string message;
            if (vm.NewParent != null)
            {
                category.Parent = vm.NewParent;
                message = string.Format("Moved '{0}'. The new parent category is '{1}'.", category, category.Parent);
            }
            else
            {
                category.Parent = null;
                message = string.Format("Made '{0}' into a top-level category.", category);
            }
            Db.SaveChanges();
            return Categories(message);
        }
        [HttpPost]
        public ActionResult DeleteCategory([Bind(Prefix = "")] EditCategoryViewModel vm)
        {
            var category = vm.Category;
            if (category.DirectChildren.Any())
            {
                return Categories(string.Format("Unable to delete '{0}', it has these child categories: {1}.", category, string.Join(", ", category.DirectChildren)));
            }
            if (Db.Transactions.Where(category.RecursiveFilter).Any())
            {
                return Categories(string.Format("Unable to delete '{0}', there are transactions associated with it.", category));
            }
            Db.Categories.Remove(category);
            Db.SaveChanges();
            return Categories(string.Format("Deleted category '{0}'", category));
        }
    }
}