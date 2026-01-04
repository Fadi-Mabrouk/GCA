using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GCA.BLL.Interfaces;
using GCA.Models;

namespace GCA.UI.ViewModels
{
    public partial class ClientViewModel : ObservableObject
    {
        private readonly IClientService _clientService;

        [ObservableProperty]
        private ObservableCollection<Client> _clients = new();

        public ClientViewModel(IClientService clientService)
        {
            _clientService = clientService;
            LoadDataCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            var data = await _clientService.GetAllClientsAsync();
            Clients = new ObservableCollection<Client>(data);
        }

        [RelayCommand]
        public async Task DeleteClientAsync(Client? client)
        {
            if (client == null) return;
            await _clientService.DeleteClientAsync(client.Id);
            await LoadDataAsync();
        }

        [RelayCommand]
        public async Task AddClientAsync()
        {
            var vm = new ClientEditorViewModel();
            var window = new Views.ClientEditorWindow { DataContext = vm };
            if (window.ShowDialog() == true)
            {
                var newClient = vm.GetClient();
                await _clientService.AddClientAsync(newClient);
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        public async Task EditClientAsync(Client? client)
        {
            if (client == null) return;

            var vm = new ClientEditorViewModel(client);
            var window = new Views.ClientEditorWindow { DataContext = vm, Title = "Edit Client" };
            
            if (window.ShowDialog() == true)
            {
                vm.UpdateClient(client);
                await _clientService.UpdateClientAsync(client);
                await LoadDataAsync();
            }
        }
    }
}
