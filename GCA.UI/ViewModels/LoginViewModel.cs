using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GCA.BLL.Interfaces;

namespace GCA.UI.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string _username = string.Empty;

        // PasswordBox binding is tricky in MVVM, usually use an attached property or pass it as parameter.
        // For simplicity, we will bind to a string (not secure for real Prod, but okay for demo) or use interface.
        // Let's use a parameter in the command for PasswordBox.

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public event Action? LoginSuccess;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        public async Task LoginAsync(object? passwordBox)
        {
            ErrorMessage = string.Empty;
            
            // Unsafe casting for demo simplicity
            if (passwordBox is not System.Windows.Controls.PasswordBox pb) return;
            string password = pb.Password;

            try
            {
                var user = await _authService.LoginAsync(Username, password);
                if (user != null)
                {
                    LoginSuccess?.Invoke();
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login failed: {ex.Message}";
            }
        }
    }
}
