using System.Windows;

namespace SteamLamp
{
    public partial class SteamMessageBox : Window
    {
        public SteamMessageBox(string message, string title = "ПОДДЕРЖКА")
        {
            InitializeComponent();
            TxtMessage.Text = message;
            TxtTitle.Text = title.ToUpper();
        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public static void Show(string message, string title = "ПОДДЕРЖКА", Window owner = null)
        {
            var msg = new SteamMessageBox(message, title);
            if (owner != null) msg.Owner = owner;
            msg.ShowDialog();
        }
    }
}