using CommunityToolkit.Mvvm.ComponentModel;
using GCA.Models;

namespace GCA.UI.ViewModels
{
    public partial class SupplierEditorViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _name = string.Empty;
        [ObservableProperty]
        private string? _contactInfo;

        public SupplierEditorViewModel(Supplier? supplier = null)
        {
            if (supplier != null)
            {
                Name = supplier.Name;
                ContactInfo = supplier.ContactInfo;
            }
        }

        public Supplier GetSupplier()
        {
            return new Supplier
            {
                Name = Name,
                ContactInfo = ContactInfo
            };
        }

        public void UpdateSupplier(Supplier supplier)
        {
            supplier.Name = Name;
            supplier.ContactInfo = ContactInfo;
        }
    }
}
