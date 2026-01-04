using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GCA.Models
{
    public class Client : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
