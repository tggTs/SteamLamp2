using System.Windows;
using System.Windows.Controls;

namespace SteamLamp
{
    public partial class LibraryPage : UserControl
    {
        public LibraryPage()
        {
            InitializeComponent();
        }

        private void GoToStore_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow main)
            {
                main.OpenStore_Click(null, null);
            }
        }
    }
}