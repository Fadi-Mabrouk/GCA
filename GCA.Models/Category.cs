using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GCA.Models
{
    public class Category : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        // Navigation
        public ICollection<Part> Parts { get; set; } = new List<Part>();
    }
}
