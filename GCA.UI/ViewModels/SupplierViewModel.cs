using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GCA.BLL.Interfaces;
using GCA.Models;

namespace GCA.UI.ViewModels
{
    public partial class SupplierViewModel : ObservableObject
    {
        private readonly ISupplierService _supplierService;

        [ObservableProperty]
        private ObservableCollection<Supplier> _suppliers = new();

        public SupplierViewModel(ISupplierService supplierService)
        {
            _supplierService = supplierService;
            LoadDataCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            var data = await _supplierService.GetAllSuppliersAsync();
            Suppliers = new ObservableCollection<Supplier>(data);
        }

        [RelayCommand]
        public async Task DeleteSupplierAsync(Supplier? supplier)
        {
            if (supplier == null) return;
            await _supplierService.DeleteSupplierAsync(supplier.Id);
            await LoadDataAsync();
        }

        [RelayCommand]
        public async Task AddSupplierAsync()
        {
            var vm = new SupplierEditorViewModel();
            var window = new Views.SupplierEditorWindow { DataContext = vm };
            if (window.ShowDialog() == true)
            {
                var newSupplier = vm.GetSupplier();
                await _supplierService.AddSupplierAsync(newSupplier);
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        public async Task EditSupplierAsync(Supplier? supplier)
        {
            if (supplier == null) return;

            var vm = new SupplierEditorViewModel(supplier);
            var window = new Views.SupplierEditorWindow { DataContext = vm, Title = "Edit Supplier" };

            if (window.ShowDialog() == true)
            {
                vm.UpdateSupplier(supplier);
                await _supplierService.UpdateSupplierAsync(supplier);
                await LoadDataAsync();
            }
        }
    }
}
