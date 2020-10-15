using FinancesSharp.Helpers;
using FinancesSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace FinancesSharp.ViewModels
{
    public class SearchForm : IConvertibleToRouteValues
    {
        public Search Search { get; set; }
        public IEnumerable<Category> Categories { get; private set; }
        public IEnumerable<Person> People { get; private set; }

        public SearchForm()
            : this(new Search()) { }
        public SearchForm(Search search)
        {
            Search = search;
        }

        public void Initialize(FinanceDb db)
        {
            Categories = db.Categories;
            People = db.People;
        }

        public RouteValueDictionary ToRouteValues()
        {
            var dict = new RouteValueDictionary();
            dict.Add(this, x => x.Search.Name);
            dict.Add(this, x => x.Search.Category, CategoryId);
            dict.Add(this, x => x.Search.IncludeSubCategories);
            dict.Add(this, x => x.Search.DateFrom);
            dict.Add(this, x => x.Search.DateTo);
            dict.Add(this, x => x.Search.Income);
            dict.Add(this, x => x.Search.AmountFrom);
            dict.Add(this, x => x.Search.AmountTo);
            dict.Add(this, x => x.Search.Person, PersonId);
            return dict;
        }

        public IQueryable<Transaction> Filter(IQueryable<Transaction> transactions)
        {
            return transactions.Where(Search.IsMatch()).OrderByDescending(x => x.Date);
        }

        public string CategoryName
        {
            get
            {
                if (Search.Category != null)
                {
                    return Search.Category.Name;
                }
                return "";
            }
        }
        public int? CategoryId
        {
            get
            {
                if (Search.Category != null)
                {
                    return Search.Category.Id;
                }
                return null;
            }
        }
        public int? PersonId
        {
            get
            {
                if (Search.Person != null)
                {
                    return Search.Person.Id;
                }
                return null;
            }
        }
    }
}