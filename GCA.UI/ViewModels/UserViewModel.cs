using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GCA.BLL.Interfaces;
using GCA.Models;

namespace GCA.UI.ViewModels
{
    public partial class UserViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        [ObservableProperty]
        private ObservableCollection<User> _users = new();

        public UserViewModel(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
            LoadDataCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            Users = new ObservableCollection<User>(users);
        }

        [RelayCommand]
        public async Task AddUserAsync()
        {
            var vm = new UserEditorViewModel();
            var window = new Views.UserEditorWindow { DataContext = vm };
            
            if (window.ShowDialog() == true)
            {
                var password = window.Password;
                if (string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Password is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    await _authService.RegisterAsync(vm.Username, password, vm.FullName, vm.Role);
                    await LoadDataAsync();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Failed to create user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        public async Task DeleteUserAsync(User? user)
        {
            if (user == null) return;
            
            // Prevent deleting self? (Ideally check current logged in user ID, but simplifying for now)
            if (user.Username == "admin")
            {
                 MessageBox.Show("Cannot delete the default admin account.", "Restricted", MessageBoxButton.OK, MessageBoxImage.Warning);
                 return;
            }

            if (MessageBox.Show($"Are you sure you want to delete user '{user.Username}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await _userService.DeleteUserAsync(user.Id);
                await LoadDataAsync();
            }
        }
    }
}
