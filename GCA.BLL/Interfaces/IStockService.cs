using System.Collections.Generic;
using System.Threading.Tasks;
using GCA.Models;

namespace GCA.BLL.Interfaces
{
    public interface IStockService
    {
        Task<IEnumerable<Part>> GetAllPartsAsync();
        Task<Part?> GetPartByIdAsync(int id);
        Task<Part> AddPartAsync(Part part);
        Task UpdatePartAsync(Part part);
        Task DeletePartAsync(int id);
        Task<IEnumerable<Part>> SearchPartsAsync(string query);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<decimal> GetTotalInventoryValueAsync();
    }
}
