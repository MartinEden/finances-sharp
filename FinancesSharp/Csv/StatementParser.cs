using FinancesSharp.Helpers;
using FinancesSharp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FinancesSharp.Csv
{
    public class StatementParser : IDisposable
    {
        private static Regex cardRegex = new Regex(@" CD (?<number>\d{4})");

        private TextReader reader;
        private IEnumerable<Transaction> transactions;
        private List<string> errors;
        private Generator<TransactionType, string> types;
        private Generator<Card, string> cards;
        private Generator<Category, string> categories;

        public StatementParser(Stream stream, FinanceDb db)
        {
            this.reader = new StreamReader(stream);
            this.types = new Generator<TransactionType, string>(db.TransactionTypes, n => new TransactionType(n), (type, name) => type.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            this.cards = new Generator<Card, string>(db.Cards, n => new Card(n.ToUpper()), (card, number) => card.Number == number.ToUpper());
            this.categories = new Generator<Category, string>(db.Categories, n => new Category(n, null), (category, name) => category.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            errors = new List<string>();
            transactions = parse().ToList();
        }

        public IEnumerable<Transaction> Transactions
        {
            get { return transactions; }
        }
        public IEnumerable<string> Errors
        {
            get { return errors; }
        }

        private IEnumerable<Transaction> parse()
        {
            reader.ReadLine(); // Skip CSV headers
            while (reader.Peek() != -1)
            {
                var trx = parseLine(reader.ReadLine());
                if (trx != null)
                    yield return trx;
            }
        }

        private Transaction parseLine(string line)
        {
            try
            {
                string[] values = line.Split(new char[] { ',' });
                string text = values[4];
                var match = cardRegex.Match(text);
                Card card = null;
                if (match.Success)
                {
                    text = text.Substring(0, match.Index);
                    string cardNumber = match.Groups["number"].Value;
                    card = cards.GetOrCreate(cardNumber);
                }

                return new Transaction(DateTime.Parse(values[0]),
                    getOrCreateTransactionType(values[1]),
                    text,
                    card,
                    parseNumber(values[6]) - parseNumber(values[5]),
                    parseNumber(values[7]),
                    categories.GetOrCreate("Uncategorised"));
            }
            catch (Exception e)
            {
                errors.Add(string.Format("Problem parsing line: '{0}'. The error was: '{1}'", line, e.Message));
                return null;
            }
        }

        private decimal parseNumber(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0M;
            }
            else
            {
                return decimal.Parse(text);
            }
        }

        private TransactionType getOrCreateTransactionType(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return types.GetOrCreate("Unknown");
            }
            else
            {
                return types.GetOrCreate(name);
            }
        }

        public void Dispose()
        {
            reader.Dispose();
        }
    }
}