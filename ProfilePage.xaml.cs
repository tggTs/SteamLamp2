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
        private void OpenFriends_Click(object sender, RoutedEventArgs e)
        {
            var friendsView = new FriendsListControl();
            friendsView.FriendsNickName.Text = this.ProfileNickname.Text;
            FriendsOverlay.Content = friendsView;
            MainProfileLayout.Visibility = Visibility.Collapsed;
            FriendsOverlay.Visibility = Visibility.Visible;
        }
    }
}