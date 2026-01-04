using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace GCA.UI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private ObservableObject? _currentViewModel;

        [ObservableProperty]
        private string _title = "GCA - Automobile Junkyard Management";

        [ObservableProperty]
        private bool _isLoggedIn = false;

        public MainViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
            // Start with Login
            NavigateToLogin();
        }

        public void NavigateToLogin()
        {
            IsLoggedIn = false;
            var vm = _serviceProvider.GetRequiredService<LoginViewModel>();
            vm.LoginSuccess += () => 
            {
                IsLoggedIn = true;
                NavigateToStock();
            };
            CurrentViewModel = vm;
        }

        [RelayCommand]
        public void NavigateToDashboard()
        {
            var vm = _serviceProvider.GetRequiredService<DashboardViewModel>();
            CurrentViewModel = vm;
            Title = "GCA - Dashboard";
        }

        [RelayCommand]
        public void NavigateToStock()
        {
            var vm = _serviceProvider.GetRequiredService<StockViewModel>();
            CurrentViewModel = vm;
            Title = "GCA - Stock Management";
        }

        [RelayCommand]
        public void NavigateToClients()
        {
            var vm = _serviceProvider.GetRequiredService<ClientViewModel>();
            CurrentViewModel = vm;
            Title = "GCA - Client Management";
        }

        [RelayCommand]
        public void NavigateToSuppliers()
        {
            var vm = _serviceProvider.GetRequiredService<SupplierViewModel>();
            CurrentViewModel = vm;
            Title = "GCA - Supplier Management";
        }

        [RelayCommand]
        public void NavigateToSales()
        {
            var vm = _serviceProvider.GetRequiredService<SaleViewModel>();
            CurrentViewModel = vm;
            Title = "GCA - Point of Sales";
        }

        [RelayCommand]
        public void NavigateToPurchases()
        {
            var vm = _serviceProvider.GetRequiredService<PurchaseViewModel>();
            CurrentViewModel = vm;
            Title = "GCA - Purchase Orders (Restock)";
        }

        [RelayCommand]
        public void NavigateToReports()
        {
            var vm = _serviceProvider.GetRequiredService<SalesHistoryViewModel>();
            CurrentViewModel = vm;
            Title = "GCA - Sales Reports";
        }

        [RelayCommand]
        public void NavigateToPurchaseHistory()
        {
            var vm = _serviceProvider.GetRequiredService<PurchaseHistoryViewModel>();
            CurrentViewModel = vm;
            Title = "GCA - Purchase History";
        }

        [RelayCommand]
        public void NavigateToUsers()
        {
            var vm = _serviceProvider.GetRequiredService<UserViewModel>();
            CurrentViewModel = vm;
            Title = "GCA - User Management";
        }

        [RelayCommand]
        public void Logout()
        {
            NavigateToLogin();
        }
    }
}
