using System.Windows;

namespace GCA.UI.Views
{
    public partial class UserEditorWindow : Window
    {
        public UserEditorWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public string Password => PasswordBox.Password;
    }
}
