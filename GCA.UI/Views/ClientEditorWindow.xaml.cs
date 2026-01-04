using System.Windows;

namespace GCA.UI.Views
{
    public partial class ClientEditorWindow : Window
    {
        public ClientEditorWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
