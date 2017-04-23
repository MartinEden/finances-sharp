using FinancesSharp.Models;
using System.Collections.Generic;
using System.Linq;

namespace FinancesSharp.ViewModels
{
    public static class CategoryTree
    {
        public static IEnumerable<object> Build(FinanceDb db)
        {
            return db.TopLevelCategories
                .ToList()
                .Select(c => new CategoryNode(c))
                .ToList();
        }
        
        private class CategoryNode
        {
            public string name { get; set; }
            public string value { get; set; }
            public IEnumerable<CategoryNode> children { get; set; }

            public CategoryNode(Category category)
            {
                name = category.Name;
                value = category.Id.ToString();
                children = category.DirectChildren.Select(child => new CategoryNode(child)).ToList();
            }
        }
    }
}