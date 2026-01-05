using System.Threading.Tasks;
using GCA.Models;

namespace GCA.BLL.Interfaces
{
    public interface IReportService
    {
        void GenerateInvoicePdf(Sale sale, string filePath);
        void GeneratePurchaseInvoicePdf(Purchase purchase, string filePath);
    }
}
