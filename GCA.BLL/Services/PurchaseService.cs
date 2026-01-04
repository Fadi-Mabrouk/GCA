using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GCA.BLL.Interfaces;
using GCA.DAL;
using GCA.Models;
using Microsoft.EntityFrameworkCore;

namespace GCA.BLL.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly GCADbContext _context;

        public PurchaseService(GCADbContext context)
        {
            _context = context;
        }

        public async Task CreatePurchaseAsync(Purchase purchase)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Update Stock
                foreach (var item in purchase.LineItems)
                {
                    var part = await _context.Parts.FindAsync(item.PartId);
                    if (part == null)
                        throw new Exception($"Part with ID {item.PartId} not found.");

                    // Increment Stock
                    part.Quantity += item.Quantity;
                    
                    // Optional: Update Part's Unit Price to reflect new cost if business rule desires?
                    // For now, we only update quantity. Cost is recorded in the Purchase record.

                    // If part was Sold, mark as Available
                    if (part.State == PartState.Sold && part.Quantity > 0)
                    {
                        part.State = PartState.Available;
                    }

                    _context.Parts.Update(part);
                }

                purchase.Date = DateTime.UtcNow;
                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Purchase>> GetAllPurchasesAsync()
        {
            return await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.LineItems)
                .ThenInclude(li => li.Part)
                .OrderByDescending(p => p.Date)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalCostAsync()
        {
            var values = await _context.Purchases.Select(p => p.TotalCost).ToListAsync();
            return values.Sum();
        }
    }
}
