using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GCA.Models;

namespace GCA.UI.ViewModels
{
    public partial class PartEditorViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _name = string.Empty;
        [ObservableProperty]
        private string? _sKU;
        [ObservableProperty]
        private decimal _unitPrice;
        [ObservableProperty]
        private int _quantity;
        
        [ObservableProperty]
        private List<Category> _categories = new();
        
        [ObservableProperty]
        private Category? _selectedCategory;

        public List<PartState> States { get; } = Enum.GetValues<PartState>().ToList();
        
        [ObservableProperty]
        private PartState _state = PartState.Available;

        public PartEditorViewModel(IEnumerable<Category> categories, Part? part = null)
        {
            Categories = categories.ToList();
            
            if (part != null)
            {
                Name = part.Name;
                SKU = part.SKU;
                UnitPrice = part.UnitPrice;
                Quantity = part.Quantity;
                State = part.State;
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == part.CategoryId);
            }
            else
            {
                SelectedCategory = Categories.FirstOrDefault();
            }
        }

        public Part GetPart()
        {
            return new Part
            {
                Name = Name,
                SKU = SKU,
                UnitPrice = UnitPrice,
                Quantity = Quantity,
                CategoryId = SelectedCategory?.Id ?? 0,
                State = State
            };
        }

        public void UpdatePart(Part part)
        {
            part.Name = Name;
            part.SKU = SKU;
            part.UnitPrice = UnitPrice;
            part.Quantity = Quantity;
            part.CategoryId = SelectedCategory?.Id ?? 0;
            part.State = State;
        }
    }
}
