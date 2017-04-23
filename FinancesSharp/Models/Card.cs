using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FinancesSharp.Models
{
    public class Card
    {
        protected Card() { }
        public Card(string number)
            : this()
        {
            Number = number;
        }

        [Key]
        public string Number { get; set; }
        public virtual Person Person { get; set; }

        public override string ToString()
        {
            if (Person == null)
            {
                return string.Format("Unknown card {0}", Number);
            }
            else
            {
                return Person.Name;
            }
        }
    }
}