using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GCA.BLL.Interfaces;
using GCA.Models;

namespace GCA.UI.ViewModels
{
    public partial class PurchaseHistoryViewModel : ObservableObject
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IReportService _reportService;

        [ObservableProperty]
        private ObservableCollection<Purchase> _purchases = new();

        [ObservableProperty]
        private Purchase? _selectedPurchase;

        public PurchaseHistoryViewModel(IPurchaseService purchaseService, IReportService reportService)
        {
            _purchaseService = purchaseService;
            _reportService = reportService;
            LoadDataCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            var data = await _purchaseService.GetAllPurchasesAsync();
            Purchases = new ObservableCollection<Purchase>(data);
        }

        [RelayCommand]
        public void PrintInvoice()
        {
            if (SelectedPurchase == null) return;

            try
            {
                var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var fileName = $"PurchaseInvoice_{SelectedPurchase.Id}_{DateTime.Now:yyyyMMddHHmm}.pdf";
                var filePath = Path.Combine(desktop, fileName);

                _reportService.GeneratePurchaseInvoicePdf(SelectedPurchase, filePath);

                MessageBox.Show($"Invoice saved to Desktop:\n{fileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to generate PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        public void ViewDetails()
        {
            if (SelectedPurchase == null) return;
            var window = new Views.PurchaseDetailsWindow { DataContext = SelectedPurchase };
            window.ShowDialog();
        }
    }
}
