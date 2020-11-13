using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FinancesSharp.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancesSharp.Models
{
    public class Person : IHasId, ISelectable
    {
        [Key]
        public int Id { get; set; }
        [Required][Index("IX_Name", IsUnique = true)]
        [Column(TypeName = "varchar")][StringLength(50)]
        public string Name { get; set; }

        public virtual IEnumerable<Card> Cards { get; set; }

        private Person() { }
        public Person(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public object Value
        {
            get { return Id; }
        }

        public object Text
        {
            get { return Name; }
        }
    }
}