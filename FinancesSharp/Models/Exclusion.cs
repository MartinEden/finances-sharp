using System.ComponentModel.DataAnnotations;

namespace FinancesSharp.Models
{
    public class Exclusion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public virtual Search Search { get; set; }
        public bool IsActive { get; set; }

        public Exclusion(Search search, bool isActive = true)
        {
            Search = search;
            IsActive = isActive;
        }
    }
}