using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FinancesSharp.Models
{
    public class Transaction
    {
        protected Transaction() { }
        public Transaction(DateTime date, TransactionType type, string description, 
            Card card, decimal amount, decimal balance, Category category)
            : this()
        {
            Date = date;
            Type = type;
            OriginalName = description;
            Card = card;
            Amount = amount;
            Balance = balance;
            Category = category;
        }

        [Key]
        public int Id { get; set; }

        public virtual DateTime Date { get; set; }

        [Required]
        public string OriginalName { get; set; }
        public string NewName { get; set; }
        [Required]
        public virtual TransactionType Type { get; set; }
        public virtual Card Card { get; set; }
        public virtual decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public virtual Category Category { get; set; }

        public string Name
        {
            get
            {
                return NewName ?? OriginalName;
            }
            set
            {
                NewName = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} on {1} by {2} in category {3} for amount {4}",
                Name, Date, Card, Category, Amount);
        }
    }
}