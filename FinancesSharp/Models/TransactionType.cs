using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancesSharp.Models
{
    public class TransactionType
    {
        protected TransactionType() { }
        public TransactionType(string name)
            : this()
        {
            Name = name;
        }

        [Key]
        public int Id { get; set; }
        [Required, Index(IsUnique = true), Column(TypeName = "varchar")] 
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public static TransactionType Parse(string text, IEnumerable<TransactionType> types)
        {
            var type = types.SingleOrDefault(t => t.Name.Equals(text, StringComparison.CurrentCultureIgnoreCase));
            if (type == null)
            {
                throw new FormatException(string.Format("Unable to parse '{0}' as a TransactionType. Possible values are: {1}",
                    text, string.Join(", ", types)));
            }
            return type;
        }
    }
}