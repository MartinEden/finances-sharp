using FinancesSharp.Models;
using System.Web.Mvc;
using System.Linq;

namespace FinancesSharp.Controllers
{
    public class CardController : FinancesControllerBase
    {
        public ActionResult Index()
        {
            return View(Db.Cards.OrderBy(c => c.Number));
        }
        public ActionResult ChangeOwner(string cardNumber, string personName)
        {
            var person = Db.People.SingleOrDefault(p => p.Name == personName) ?? new Person(personName);
            var card = Db.Cards.Single(c => c.Number == cardNumber);
            card.Person = person;
            Db.SaveChanges();
            return Json("OK");
        }

    }
}
