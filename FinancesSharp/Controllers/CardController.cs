using FinancesSharp.Models;
using System.Web.Mvc;
using System.Linq;
using System;

namespace FinancesSharp.Controllers
{
    public class CardController : FinancesControllerBase
    {
        public ActionResult Index()
        {
            return View(Db.Cards.OrderBy(c => c.Number).ToList());
        }
        [HttpPost]
        public ActionResult ChangeOwner(string cardNumber, string personName)
        {
            var person = Db.People.SingleOrDefault(p => p.Name == personName) ?? new Person(personName);
            var card = Db.Cards.SingleOrDefault(c => c.Number == cardNumber);
            if (card == null)
            {
                throw new Exception("Unknown card identified by number " + cardNumber);
            }
            card.Person = person;
            Db.SaveChanges();
            return Json("OK");
        }
    }
}
