using System;
using System.Collections.Generic;
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
    public partial class CartItem : ObservableObject
    {
        public Part Part { get; set; } = null!;
        public string PartName => Part.Name;
        public decimal UnitPrice => Part.UnitPrice;
        
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalPrice))]
        private int _quantity;

        public decimal TotalPrice => Quantity * UnitPrice;
    }

    public partial class SaleViewModel : ObservableObject
    {
        private readonly ISaleService _saleService;
        private readonly IStockService _stockService;
        private readonly IClientService _clientService;

        [ObservableProperty]
        private ObservableCollection<Part> _availableParts = new();

        [ObservableProperty]
        private ObservableCollection<Client> _clients = new();

        [ObservableProperty]
        private ObservableCollection<CartItem> _cart = new();

        [ObservableProperty]
        private Client? _selectedClient;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanCheckout))]
        private decimal _grandTotal;

        public bool CanCheckout => Cart.Any() && SelectedClient != null;

        public SaleViewModel(ISaleService saleService, IStockService stockService, IClientService clientService)
        {
            _saleService = saleService;
            _stockService = stockService;
            _clientService = clientService;

            LoadDataCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            var parts = await _stockService.SearchPartsAsync(SearchQuery);
            AvailableParts = new ObservableCollection<Part>(parts.Where(p => p.Quantity > 0));

            var clients = await _clientService.GetAllClientsAsync();
            Clients = new ObservableCollection<Client>(clients);
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
                if (existing.Quantity < part.Quantity)
                {
                    existing.Quantity++;
                }
            }
            else
            {
                Cart.Add(new CartItem { Part = part, Quantity = 1 });
            }
            RecalculateTotal();
        }

        [RelayCommand]
        public void RemoveFromCart(CartItem? item)
        {
            if (item == null) return;
            Cart.Remove(item);
            RecalculateTotal();
        }

        private void RecalculateTotal()
        {
            GrandTotal = Cart.Sum(i => i.TotalPrice);
            OnPropertyChanged(nameof(CanCheckout));
        }

        [RelayCommand]
        public async Task CheckoutAsync()
        {
            if (!CanCheckout || SelectedClient == null) return;

            // Build Sale Entity
            var sale = new Sale
            {
                ClientId = SelectedClient.Id,
                LineItems = Cart.Select(c => new SaleLineItem
                {
                    PartId = c.Part.Id,
                    Quantity = c.Quantity,
                    UnitPriceSnapshot = c.UnitPrice
                }).ToList()
            };

            try
            {
                await _saleService.CreateSaleAsync(sale);
                MessageBox.Show("Sale completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearCart();
                await LoadDataAsync(); // Refresh stock
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sale failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearCart()
        {
            Cart.Clear();
            RecalculateTotal();
            SelectedClient = null;
        }
    }
}
