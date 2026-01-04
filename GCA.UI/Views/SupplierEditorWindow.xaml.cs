using System.Windows;

namespace GCA.UI.Views
{
    public partial class SupplierEditorWindow : Window
    {
        public SupplierEditorWindow()
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
