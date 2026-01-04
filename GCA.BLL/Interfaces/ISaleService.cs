using System.Collections.Generic;
using System.Threading.Tasks;
using GCA.Models;

namespace GCA.BLL.Interfaces
{
    public interface ISaleService
    {
        Task CreateSaleAsync(Sale sale);
        Task<IEnumerable<Sale>> GetSalesHistoryAsync();
        Task<decimal> GetTotalRevenueAsync();
    }
}
