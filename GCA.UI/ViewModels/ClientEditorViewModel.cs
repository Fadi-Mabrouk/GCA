using CommunityToolkit.Mvvm.ComponentModel;
using GCA.Models;

namespace GCA.UI.ViewModels
{
    public partial class ClientEditorViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _name = string.Empty;
        [ObservableProperty]
        private string? _phone;
        [ObservableProperty]
        private string? _email;
        [ObservableProperty]
        private string? _address;

        public ClientEditorViewModel(Client? client = null)
        {
            if (client != null)
            {
                Name = client.Name;
                Phone = client.Phone;
                Email = client.Email;
                Address = client.Address;
            }
        }

        public Client GetClient()
        {
            return new Client
            {
                Name = Name,
                Phone = Phone,
                Email = Email,
                Address = Address
            };
        }

        public void UpdateClient(Client client)
        {
            client.Name = Name;
            client.Phone = Phone;
            client.Email = Email;
            client.Address = Address;
        }
    }
}
