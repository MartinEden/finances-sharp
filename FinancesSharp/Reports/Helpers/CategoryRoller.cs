using FinancesSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FinancesSharp.Reports.Helpers
{
    public abstract class CategoryRoller<TSource>
        where TSource : IHasCategory
    {
        public readonly Category TopLevelCategory;            

        public CategoryRoller(Category topLevelCategory)
        {
            TopLevelCategory = topLevelCategory;
        }

        public IEnumerable<CategoryAmount> RollUpByCategoriesOfInterest(FinanceDb db, IEnumerable<TSource> totals)
        {
            initialize(totals);
            foreach (var parent in getCategoriesOfInterest(db))
            {
                var children = new HashSet<Category>(parent.Children);
                var categoryTotals = totals.Where(t => children.Contains(t.Category));
                yield return new CategoryAmount(parent, rollUpCategory(categoryTotals));
            }
            if (TopLevelCategory != null)
            {
                var categoryTotals = totals.Where(t => t.Category == TopLevelCategory);
                yield return new CategoryAmount(TopLevelCategory, rollUpCategory(categoryTotals));
            }
        }

        protected virtual void initialize(IEnumerable<TSource> totals) { }
        protected abstract decimal rollUpCategory(IEnumerable<TSource> categoryTotals);

        protected IEnumerable<Category> getCategoriesOfInterest(FinanceDb db)
        {
            if (TopLevelCategory == null)
            {
                return db.TopLevelCategories;
            }
            else
            {
                return TopLevelCategory.DirectChildren;
            }
        }
    }
}