using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FinancesSharp.Helpers
{
    public class Generator<TItem, TKey>
        where TItem : class
    {
        private DbSet<TItem> set;
        private List<TItem> existingItems;
        private List<TItem> newItems;
        private Func<TItem, TKey, bool> equalitycheck;
        private Func<TKey, TItem> maker;

        public Generator(DbSet<TItem> set, Func<TKey, TItem> maker, Func<TItem, TKey, bool> equalitycheck)
        {
            this.set = set;
            this.existingItems = set.ToList();
            this.newItems = new List<TItem>();
            this.maker = maker;
            this.equalitycheck = equalitycheck;
        }

        public TItem GetOrCreate(TKey key)
        {
            var item = existingItems.Concat(newItems).SingleOrDefault(i => equalitycheck(i, key));
            if (object.Equals(item, null))
            {
                item = maker(key);
                set.Add(item);
                newItems.Add(item);
            }
            return item;
        }
    }
}