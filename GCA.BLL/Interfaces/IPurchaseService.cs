using System.Threading.Tasks;
using GCA.Models;

namespace GCA.BLL.Interfaces
{
    public interface IPurchaseService
    {
        Task CreatePurchaseAsync(Purchase purchase);
        Task<IEnumerable<Purchase>> GetAllPurchasesAsync();
        Task<decimal> GetTotalCostAsync();
    }
}
