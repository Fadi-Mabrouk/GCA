using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GCA.BLL.Interfaces;
using GCA.Models;

namespace GCA.UI.ViewModels
{
    public partial class PurchaseHistoryViewModel : ObservableObject
    {
        private readonly IPurchaseService _purchaseService;

        [ObservableProperty]
        private ObservableCollection<Purchase> _purchases = new();

        [ObservableProperty]
        private Purchase? _selectedPurchase;

        public PurchaseHistoryViewModel(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
            LoadDataCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            var data = await _purchaseService.GetAllPurchasesAsync();
            Purchases = new ObservableCollection<Purchase>(data);
        }
    }
}
