using System;
using System.Windows;
using System.Windows.Controls;

namespace SteamLamp
{
    public partial class ProfilePage : UserControl
    {
        public ProfilePage()
        {
            InitializeComponent();
            if (Session.CurrentUser != null)
            {
                ProfileNickname.Text = Session.CurrentUser.Nickname;
            }
        }
        private void GoToStore_Internal_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow main)
            {
                main.OpenStore_Click(null, null);
            }
        }
    }
}