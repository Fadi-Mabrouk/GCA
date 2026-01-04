using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCA.Models
{
    public class Part : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? SKU { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public PartState State { get; set; } = PartState.Available;
    }
}
