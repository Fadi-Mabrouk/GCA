using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GCA.Models;

namespace GCA.UI.ViewModels
{
    public partial class UserEditorViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _username = string.Empty;
        
        [ObservableProperty]
        private string _fullName = string.Empty;

        public List<UserRole> Roles { get; } = Enum.GetValues<UserRole>().ToList();

        [ObservableProperty]
        private UserRole _role = UserRole.Employee;
    }
}
