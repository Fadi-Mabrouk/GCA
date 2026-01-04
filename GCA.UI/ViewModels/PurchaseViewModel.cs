using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GCA.BLL.Interfaces;
using GCA.Models;

namespace GCA.UI.ViewModels
{
    // Reusing partial CartItem concept but for PurchaseCartItem to track Cost instead of Price
    public partial class PurchaseCartItem : ObservableObject
    {
        public Part Part { get; set; } = null!;
        public string PartName => Part.Name;
        
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalCost))]
        private decimal _unitCost;
        
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalCost))]
        private int _quantity;

        public decimal TotalCost => Quantity * UnitCost;
    }

    public partial class PurchaseViewModel : ObservableObject
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IStockService _stockService;
        private readonly ISupplierService _supplierService;

        [ObservableProperty]
        private ObservableCollection<Part> _availableParts = new();

        [ObservableProperty]
        private ObservableCollection<Supplier> _suppliers = new();

        [ObservableProperty]
        private ObservableCollection<PurchaseCartItem> _cart = new();

        [ObservableProperty]
        private Supplier? _selectedSupplier;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanCheckout))]
        private decimal _grandTotal;

        public bool CanCheckout => Cart.Any() && SelectedSupplier != null;

        public PurchaseViewModel(IPurchaseService purchaseService, IStockService stockService, ISupplierService supplierService)
        {
            _purchaseService = purchaseService;
            _stockService = stockService;
            _supplierService = supplierService;

            LoadDataCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            var parts = await _stockService.SearchPartsAsync(SearchQuery);
            AvailableParts = new ObservableCollection<Part>(parts);

            var suppliers = await _supplierService.GetAllSuppliersAsync();
            Suppliers = new ObservableCollection<Supplier>(suppliers);
        }

        [RelayCommand]
        public async Task SearchAsync()
        {
            await LoadDataAsync();
        }

        [RelayCommand]
        public void AddToCart(Part? part)
        {
            if (part == null) return;

            var existing = Cart.FirstOrDefault(c => c.Part.Id == part.Id);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                // Default cost to current unit price as a suggestion, user can edit
                Cart.Add(new PurchaseCartItem { Part = part, Quantity = 1, UnitCost = part.UnitPrice });
            }
            RecalculateTotal();
        }

        [RelayCommand]
        public void RemoveFromCart(PurchaseCartItem? item)
        {
            if (item == null) return;
            Cart.Remove(item);
            RecalculateTotal();
        }

        private void RecalculateTotal()
        {
            GrandTotal = Cart.Sum(i => i.TotalCost);
            OnPropertyChanged(nameof(CanCheckout));
        }

        [RelayCommand]
        public async Task CheckoutAsync()
        {
            if (!CanCheckout || SelectedSupplier == null) return;

            var purchase = new Purchase
            {
                SupplierId = SelectedSupplier.Id,
                TotalCost = GrandTotal,
                LineItems = Cart.Select(c => new PurchaseLineItem
                {
                    PartId = c.Part.Id,
                    Quantity = c.Quantity,
                    UnitCost = c.UnitCost
                }).ToList()
            };

            try
            {
                await _purchaseService.CreatePurchaseAsync(purchase);
                MessageBox.Show("Purchase Order processed successfully! Stock updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearCart();
                await LoadDataAsync(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Purchase failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearCart()
        {
            Cart.Clear();
            RecalculateTotal();
            SelectedSupplier = null;
        }
    }
}
