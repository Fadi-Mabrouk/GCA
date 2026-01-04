using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GCA.BLL.Interfaces;
using GCA.DAL;
using GCA.Models;
using Microsoft.EntityFrameworkCore;

namespace GCA.BLL.Services
{
    public class StockService : IStockService
    {
        private readonly GCADbContext _context;

        public StockService(GCADbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Part>> GetAllPartsAsync()
        {
            return await _context.Parts
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Part?> GetPartByIdAsync(int id)
        {
            return await _context.Parts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Part> AddPartAsync(Part part)
        {
            _context.Parts.Add(part);
            await _context.SaveChangesAsync();
            return part;
        }

        public async Task UpdatePartAsync(Part part)
        {
            var existing = await _context.Parts.FindAsync(part.Id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(part);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeletePartAsync(int id)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part != null)
            {
                _context.Parts.Remove(part);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Part>> SearchPartsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await GetAllPartsAsync();

            query = query.ToLower();
            return await _context.Parts
                .Include(p => p.Category)
                .Where(p => p.Name.ToLower().Contains(query) || 
                            (p.SKU != null && p.SKU.ToLower().Contains(query)) ||
                            p.Category.Name.ToLower().Contains(query))
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            var values = await _context.Parts.Select(p => (decimal)p.Quantity * p.UnitPrice).ToListAsync();
            return values.Sum();
        }
    }
}
