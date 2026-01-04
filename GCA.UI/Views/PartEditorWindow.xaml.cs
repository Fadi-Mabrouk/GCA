using System.Windows;

namespace GCA.UI.Views
{
    public partial class PartEditorWindow : Window
    {
        public PartEditorWindow()
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
