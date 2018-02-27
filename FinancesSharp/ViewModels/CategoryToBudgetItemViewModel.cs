using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FinancesSharp.Models;

namespace FinancesSharp.ViewModels
{
    public class CategoryToBudgetItemViewModel
    {
        [Required, DisplayName("Budget item")]
        public int ItemId { get; set; }
        [Required, DisplayName("Category")]
        public int CategoryId { get; set; }
    }
}