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
    public class SaleService : ISaleService
    {
        private readonly GCADbContext _context;

        public SaleService(GCADbContext context)
        {
            _context = context;
        }

        public async Task CreateSaleAsync(Sale sale)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Validate and Deduct Stock
                foreach (var item in sale.LineItems)
                {
                    var part = await _context.Parts.FindAsync(item.PartId);
                    if (part == null) 
                        throw new Exception($"Part with ID {item.PartId} not found.");

                    if (part.Quantity < item.Quantity)
                        throw new Exception($"Insufficient stock for part '{part.Name}'. Requested: {item.Quantity}, Available: {part.Quantity}");

                    // Determine unit price snapshot if not set
                    if (item.UnitPriceSnapshot == 0)
                        item.UnitPriceSnapshot = part.UnitPrice;

                    // Deduct stock
                    part.Quantity -= item.Quantity;
                    
                    // Update state if sold out (optional business rule)
                    if (part.Quantity == 0)
                        part.State = PartState.Sold;

                    _context.Parts.Update(part);
                }

                // Calculate Total
                sale.TotalAmount = sale.LineItems.Sum(li => li.Quantity * li.UnitPriceSnapshot);
                sale.Date = DateTime.UtcNow;

                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Sale>> GetSalesHistoryAsync()
        {
            return await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.LineItems)
                .ThenInclude(li => li.Part)
                .OrderByDescending(s => s.Date)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            var values = await _context.Sales.Select(s => s.TotalAmount).ToListAsync();
            return values.Sum();
        }
    }
}
