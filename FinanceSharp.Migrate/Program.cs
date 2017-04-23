using FinancesSharp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace FinancesSharp.Migrate
{
    /// <summary>
    /// This project reads in the SQLite database from the old finances app and inserts it into the database
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            string dbPath = args[0];
            new Program().Run(dbPath);
        }

        private Mapper mapper;
        private FinanceDb db;
        private SQLiteDatabase litedb;

        public void Run(string dbPath)
        {
            mapper = new Mapper();
            using (db = new FinanceDb())
            {
                using (var trx = db.Database.BeginTransaction())
                {
                    using (litedb = new SQLiteDatabase(dbPath))
                    {
                        var rows = litedb.Query("SELECT name FROM sqlite_master WHERE type = 'table'");
                        printRows(rows);

                        migratePeople();
                        var cards = migrateCards();
                        migrateCategories();
                        migrateSearches();
                        migrateRules();
                        migrateExclusions();
                        migrateTransactions(cards);
                    }
                    db.SaveChanges();
                    trx.Commit();
                }
            }
        }

        private void migratePeople()
        {
            var rows = litedb.Query("SELECT id, name FROM finances_app_person");
            foreach (var row in rows)
            {
                var person = new Person((string)row[1]);
                db.People.Add(person);
                mapper.RecordMapping((long)row[0], person);
                Console.WriteLine("Migrated person '{0}'", person);
            }
        }
        private IEnumerable<Card> migrateCards()
        {
            var rows = litedb.Query("SELECT number, person_id FROM finances_app_card");
            var cards = new List<Card>();
            foreach (var row in rows)
            {
                var card = new Card((string)row[0]);
                card.Person = mapper.Map<Person>((long)row[1]);
                Console.WriteLine("Migrated card '{0}' for '{1}'", card.Number, card.Person);
                cards.Add(card);
            }
            return cards;
        }
        private void migrateCategories()
        {
            var rows = litedb.Query("SELECT id, name, in_category_id FROM finances_app_category");
            foreach (var row in rows)
            {
                var category = new Category((string)row[1], null);
                db.Categories.Add(category);
                mapper.RecordMapping((long)row[0], category);
            }
            foreach (var row in rows)
            {
                var category = mapper.Map<Category>((long)row[0]);
                if (row[2] != DBNull.Value)
                {
                    category.Parent = mapper.Map<Category>((long)row[2]);
                }
                Console.WriteLine("Migrated category '{0}' with parent '{1}'", category, category.Parent);
            }
        }
        private void migrateTransactions(IEnumerable<Card> cards)
        {
            var rows = litedb.Query("SELECT id, date, raw_name, name, transaction_type, card_id, amount, balance, category_id FROM finances_app_transaction");
            var types = migrateTransactionTypes(rows);

            int count = rows.Count();
            int done = 0;
            Console.WriteLine("Beginning migrating {0} transactions", count);
            foreach (var row in rows)
            {
                var transaction = new Transaction(
                    (DateTime)row[1],                              // Date
                    types[(string)row[4]],                         // Type
                    (string)row[2],                                // OriginalName
                    cards.SingleOrDefault(c => c.Number == cast<string>(row[5])), // Card
                    (decimal)row[6],                               // Amount
                    (decimal)row[7],                               // Balance
                    mapper.Map<Category>((long)row[8]));           // Category
                transaction.Name = (string)row[3];
                db.Transactions.Add(transaction);
                mapper.RecordMapping((long)row[0], transaction);
                done++;
                if (done % 100 == 0)
                {
                    db.SaveChanges();
                    Console.WriteLine("{0} remaining", count - done);
                }
            }
            Console.WriteLine("Migrated {0} transactions", count);
        }
        private Dictionary<string, TransactionType> migrateTransactionTypes(IEnumerable<object[]> rows)
        {
            var types = new Dictionary<string, TransactionType>();
            types[""] = new TransactionType("Unknown");
            foreach (var row in rows)
            {
                string name = (string)row[4];
                if (!string.IsNullOrWhiteSpace(name))
                {
                    var type = new TransactionType(name);
                    types[name] = type;
                }
            }
            Console.WriteLine("Migrated transaction types: {0}", string.Join(", ", types.Values));
            return types;
        }

        private void migrateSearches()
        {
            var rows = litedb.Query("SELECT id, name, category_id, date_from, date_to, income, amount_from, amount_to, who_id FROM finances_app_search");
            foreach (var row in rows)
            {
                var search = new Search()
                {
                    Name = cast<string>(row[1]),
                    Category = mapper.MapIfNotNull<Category>(row[2]),
                    DateFrom = cast<DateTime?>(row[3]),
                    DateTo = cast<DateTime?>(row[4]),
                    Income = cast<bool?>(row[5]),
                    AmountFrom = cast<decimal?>(row[6]),
                    AmountTo = cast<decimal?>(row[7]),
                    Person = mapper.MapIfNotNull<Person>(row[8]),
                };
                db.Searches.Add(search);
                mapper.RecordMapping((long)row[0], search);
                Console.WriteLine("Migrated search '{0}'", search);
            }
        }
        private void migrateRules()
        {
            var rows = litedb.Query("SELECT id, criteria_id, categorisation_id FROM finances_app_auto_categorise");
            foreach (var row in rows)
            {
                var rule = new CategorisationRule(mapper.Map<Search>((long)row[1]), mapper.Map<Category>((long)row[2]));
                db.CategorisationRules.Add(rule);
                mapper.RecordMapping((long)row[0], rule);
                Console.WriteLine("Migrated auto categorisation rule '{0}'", rule);
            }
        }
        private void migrateExclusions()
        {
            var rows = litedb.Query("SELECT id, criteria_id, active FROM finances_app_exclusion");
            foreach (var row in rows)
            {
                var exclusion = new Exclusion(mapper.Map<Search>((long)row[1]), isActive: (bool)row[2]);
                db.Exclusions.Add(exclusion);
                mapper.RecordMapping((long)row[0], exclusion);
                Console.WriteLine("Migrated {0} exclusion: {1}", exclusion.IsActive ? "  active" : "inactive", exclusion.Search);
            }
        }

        private T cast<T>(object value)
        {
            if (value == DBNull.Value)
            {
                return default(T);
            }
            return (T)value;
        }

        private void printSqlForTable(string tableName)
        {
            Console.WriteLine("SQL for table {0}", tableName);
            printRows(litedb.Query(string.Format("SELECT sql FROM sqlite_master WHERE tbl_name = '{0}' AND type = 'table'", tableName)));
        }
        private void printTableContents(string tableName)
        {
            printRows(litedb.Query(string.Format("SELECT * FROM {0}", tableName)));
        }
        private static void printRows(IEnumerable<IEnumerable<object>> rows)
        {
            foreach (var row in rows)
            {
                Console.WriteLine(string.Join(", ", row));
            }
        }
    }
}
