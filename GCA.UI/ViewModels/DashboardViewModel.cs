using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GCA.BLL.Interfaces;

namespace GCA.UI.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IStockService _stockService;
        private readonly ISaleService _saleService;
        private readonly IPurchaseService _purchaseService;

        [ObservableProperty]
        private decimal _totalInventoryValue;

        [ObservableProperty]
        private decimal _totalRevenue;

        [ObservableProperty]
        private decimal _totalPurchaseCost;

        [ObservableProperty]
        private decimal _netProfit;

        public DashboardViewModel(IStockService stockService, ISaleService saleService, IPurchaseService purchaseService)
        {
            _stockService = stockService;
            _saleService = saleService;
            _purchaseService = purchaseService;
            RefreshCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        public async Task RefreshAsync()
        {
            TotalInventoryValue = await _stockService.GetTotalInventoryValueAsync();
            TotalRevenue = await _saleService.GetTotalRevenueAsync();
            TotalPurchaseCost = await _purchaseService.GetTotalCostAsync();
            NetProfit = TotalRevenue - TotalPurchaseCost;
        }
    }
}
