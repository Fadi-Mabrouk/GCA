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
    public partial class SalesHistoryViewModel : ObservableObject
    {
        private readonly ISaleService _saleService;
        private readonly IReportService _reportService;

        [ObservableProperty]
        private ObservableCollection<Sale> _sales = new();

        [ObservableProperty]
        private Sale? _selectedSale;

        public SalesHistoryViewModel(ISaleService saleService, IReportService reportService)
        {
            _saleService = saleService;
            _reportService = reportService;
            LoadDataCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            var data = await _saleService.GetSalesHistoryAsync();
            Sales = new ObservableCollection<Sale>(data);
        }

        [RelayCommand]
        public void PrintInvoice()
        {
            if (SelectedSale == null) return;

            try
            {
                var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var fileName = $"Invoice_{SelectedSale.Id}_{DateTime.Now:yyyyMMddHHmm}.pdf";
                var filePath = Path.Combine(desktop, fileName);

                _reportService.GenerateInvoicePdf(SelectedSale, filePath);

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
            if (SelectedSale == null) return;
            var window = new Views.SaleDetailsWindow { DataContext = SelectedSale };
            window.ShowDialog();
        }
    }
}
