using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GCA.BLL.Interfaces;
using GCA.Models;

namespace GCA.UI.ViewModels
{
    public partial class StockViewModel : ObservableObject
    {
        private readonly IStockService _stockService;

        [ObservableProperty]
        private ObservableCollection<Part> _parts = new();

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        public StockViewModel(IStockService stockService)
        {
            _stockService = stockService;
            LoadDataCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            var data = await _stockService.SearchPartsAsync(SearchQuery);
            Parts = new ObservableCollection<Part>(data);
        }

        [RelayCommand]
        public async Task SearchAsync()
        {
            await LoadDataAsync();
        }

        [RelayCommand]
        public async Task DeletePartAsync(Part? part)
        {
            if (part == null) return;
            await _stockService.DeletePartAsync(part.Id);
            await LoadDataAsync();
        }

        [RelayCommand]
        public async Task AddPartAsync()
        {
            var categories = await _stockService.GetAllCategoriesAsync();
            var vm = new PartEditorViewModel(categories);
            
            var window = new Views.PartEditorWindow { DataContext = vm };
            if (window.ShowDialog() == true)
            {
                var newPart = vm.GetPart();
                await _stockService.AddPartAsync(newPart);
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        public async Task EditPartAsync(Part? part)
        {
            if (part == null) return;

            var categories = await _stockService.GetAllCategoriesAsync();
            var vm = new PartEditorViewModel(categories, part);

            var window = new Views.PartEditorWindow { DataContext = vm };
            if (window.ShowDialog() == true)
            {
                vm.UpdatePart(part);
                await _stockService.UpdatePartAsync(part);
                await LoadDataAsync();
            }
        }
    }
}
