using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCA.Models
{
    public class Purchase : BaseEntity
    {
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }

        public ICollection<PurchaseLineItem> LineItems { get; set; } = new List<PurchaseLineItem>();
    }

    public class PurchaseLineItem : BaseEntity
    {
        public int PurchaseId { get; set; }
        [ForeignKey("PurchaseId")]
        public Purchase Purchase { get; set; } = null!;

        public int PartId { get; set; }
        [ForeignKey("PartId")]
        public Part Part { get; set; } = null!;

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }

        [NotMapped]
        public decimal Total => Quantity * UnitCost;
    }
}
