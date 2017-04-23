using FinancesSharp.Models;
using FinancesSharp.Reports;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FinancesSharp.Tests
{
    public class AverageMonthlySpendingByCategoryTests
    {
        private Category catA;
        private Category catB;
        private DateTime jan;
        private DateTime feb;
        private DateTime mar;

        [SetUp]
        public void SetUp()
        {
            catA = new Category("A", null);
            catB = new Category("B", null);

            jan = new DateTime(2016, 01, 01);
            feb = new DateTime(2016, 02, 01);
            mar = new DateTime(2016, 03, 01);
        }

        [Test]
        public void CanCalculateAverageForOneCategoryOverTwoMonths()
        {
            var transactions = new List<Transaction>()
            {
                // £10 in January
                makeTransaction(catA, jan, -4),
                makeTransaction(catA, jan, -6),
                // £20 in February
                makeTransaction(catA, feb, -18),
                makeTransaction(catA, feb, -2),
            };
            var db = makeMockDb(transactions);
            var result = getAverageSpendingByCategory(db, jan, feb);
            Assert.AreEqual(15m, result[catA]);
        }
        [Test]
        public void CanCalculateAverageForTwoCategories()
        {
            var transactions = new List<Transaction>()
            {
                makeTransaction(catA, jan, -1),
                makeTransaction(catA, feb, -1),

                makeTransaction(catB, jan, -2),
                makeTransaction(catB, feb, -4),
            };
            var db = makeMockDb(transactions);
            var result = getAverageSpendingByCategory(db, jan, feb);
            Assert.AreEqual(1m, result[catA]);
            Assert.AreEqual(3m, result[catB]);
        }
        [Test]
        public void CanCalculateAverageForCategoryWithGapInSpending()
        {
            var transactions = new List<Transaction>()
            {
                makeTransaction(catA, jan, -10),
                makeTransaction(catA, mar, -20),
            };
            var db = makeMockDb(transactions);
            var result = getAverageSpendingByCategory(db, jan, mar);
            Assert.AreEqual(10m, result[catA]);
        }
        [Test]
        public void CanCalculateAverageForCategoryWithSubcategories()
        {
            var parent = new Category("Parent", null);
            var childA = new Category("Child A", parent);
            var childB = new Category("Child B", parent);
            var childAA = new Category("Child A.A", childA);

            var transactions = new List<Transaction>()
            {
                makeTransaction(parent, jan, -10),
                makeTransaction(childA, feb, -20),
                makeTransaction(childB, mar, -20),
                makeTransaction(childAA, mar, -40),
            };
            var db = makeMockDb(transactions);
            var result = getAverageSpendingByCategory(db, jan, mar);
            Assert.AreEqual(30m, result[parent]);
        }

        private Dictionary<Category, decimal> getAverageSpendingByCategory(FinanceDb db, DateTime startDate, DateTime endDate)
        {
            var report = new AverageMonthlySpendingByCategory();
            report.DateFrom = startDate;
            report.DateTo = endDate;
            report.Run(db);
            return report.Data.ToDictionary(ca => ca.Category, ca => ca.Amount);
        }

        private FinanceDb makeMockDb(IEnumerable<Transaction> transactions)
        {
            var db = new Mock<FinanceDb>();
            db.Setup(x => x.Transactions).Returns(makeMockDbSet(transactions.ToList()));
            var allCategories = transactions.Select(t => t.Category).Distinct().ToList();
            foreach (var category in allCategories)
            {
                if (category.Parent != null)
                {
                    category.Parent.DirectChildren.Add(category);
                }
            }

            db.Setup(x => x.Categories).Returns(makeMockDbSet(allCategories));
            return db.Object;
        }

        private DbSet<T> makeMockDbSet<T>(List<T> data)
            where T : class
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            mockSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockSet.Object);
            return mockSet.Object;
        }
        private Transaction makeTransaction(Category category, DateTime date, decimal amount)
        {
            var trx = new Mock<Transaction>();
            trx.Setup(x => x.Category).Returns(category);
            trx.Setup(x => x.Date).Returns(date);
            trx.Setup(x => x.Amount).Returns(amount);
            return trx.Object;
        }
    }
}
