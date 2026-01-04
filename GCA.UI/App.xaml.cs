using System;
using System.Windows;
using GCA.BLL.Interfaces;
using GCA.BLL.Services;
using GCA.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GCA.UI.ViewModels;

namespace GCA.UI
{
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; }

        public App()
        {
            Services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // DbContext
            services.AddDbContext<GCADbContext>(options =>
            {
                options.UseSqlite("Data Source=gca.db");
            });

            // Services
            services.AddTransient<DataSeederService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<ISaleService, SaleService>();
            services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IUserService, UserService>();

            // ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<StockViewModel>();
            services.AddTransient<ClientViewModel>();
            services.AddTransient<SupplierViewModel>();
            services.AddTransient<SaleViewModel>();
            services.AddTransient<PurchaseViewModel>();
            services.AddTransient<SalesHistoryViewModel>();
            services.AddTransient<PurchaseHistoryViewModel>();
            services.AddTransient<UserViewModel>();

            // Views (MainWindow)
            services.AddTransient<MainWindow>();

            services.AddTransient<ClientViewModel>();
            services.AddTransient<SupplierViewModel>();

            return services.BuildServiceProvider();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Seed Data
            var seeder = Services.GetRequiredService<DataSeederService>();
            await seeder.SeedAsync();

            // Show Main Window
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
