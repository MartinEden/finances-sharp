using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancesSharp.ViewModels
{
    public class ChangeCategoryResult
    {
        public bool IsNewCategory { get; set; }
        public string CategoryName { get; set; }

        public static ChangeCategoryResult OK()
        {
            return new ChangeCategoryResult()
            {
                IsNewCategory = false,
                CategoryName = null,
            };
        }

        public static ChangeCategoryResult NewCategory(Models.Category category)
        {
            return new ChangeCategoryResult()
            {
                IsNewCategory = true,
                CategoryName = category.Name,
            };
        }
    }
}