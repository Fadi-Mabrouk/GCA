using System.Collections.Generic;
using System.Threading.Tasks;
using GCA.BLL.Interfaces;
using GCA.DAL;
using GCA.Models;
using Microsoft.EntityFrameworkCore;

namespace GCA.BLL.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly GCADbContext _context;

        public SupplierService(GCADbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.Suppliers.ToListAsync();
        }

        public async Task AddSupplierAsync(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSupplierAsync(Supplier supplier)
        {
            var existing = await _context.Suppliers.FindAsync(supplier.Id);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(supplier);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }
        }
    }
}
