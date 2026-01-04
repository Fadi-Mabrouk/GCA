using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GCA.Models
{
    public class Supplier : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? ContactInfo { get; set; }

        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }
}
