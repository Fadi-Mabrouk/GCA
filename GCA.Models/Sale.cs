using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCA.Models
{
    public class Sale : BaseEntity
    {
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client Client { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public ICollection<SaleLineItem> LineItems { get; set; } = new List<SaleLineItem>();
    }

    public class SaleLineItem : BaseEntity
    {
        public int SaleId { get; set; }
        [ForeignKey("SaleId")]
        public Sale Sale { get; set; } = null!;

        public int PartId { get; set; }
        [ForeignKey("PartId")]
        public Part Part { get; set; } = null!;

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPriceSnapshot { get; set; } // Price at moment of sale

        [NotMapped]
        public decimal Total => Quantity * UnitPriceSnapshot;
    }
}
