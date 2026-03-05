using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SteamLamp
{
    public partial class ProfilePage : UserControl
    {
        public ProfilePage()
        {
            InitializeComponent();
            LoadUserData();
        }
        public void LoadUserData()
        {
            if (Session.CurrentUser != null)
            {
                ProfileNickname.Text = Session.CurrentUser.Nickname;
                ProfileBio.Text = string.IsNullOrWhiteSpace(Session.CurrentUser.Bio)
                    ? "Описание профиля не заполнено."
                    : Session.CurrentUser.Bio;
                LoadFriendsPreview();
            }
        }

        private void LoadFriendsPreview()
        {
            if (Session.CurrentUser == null) return;
            FriendsPreviewContainer.Children.Clear();

            using (var db = new AppDbContext())
            {
                int myId = Session.CurrentUser.Id;
                var myFriends = db.Friends
                                  .Where(f => f.UserId == myId)
                                  .Take(5)
                                  .ToList();

                foreach (var friend in myFriends)
                {
                    Grid fGrid = new Grid { Margin = new Thickness(0, 0, 0, 8) };
                    fGrid.Children.Add(new Border
                    {
                        Width = 30,
                        Height = 30,
                        Background = new SolidColorBrush(Color.FromRgb(102, 192, 244)),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        CornerRadius = new CornerRadius(2)
                    });
                    fGrid.Children.Add(new TextBlock
                    {
                        Text = friend.FriendNickname,
                        Foreground = new SolidColorBrush(Color.FromRgb(102, 192, 244)),
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(40, 0, 0, 0),
                        FontSize = 12
                    });

                    FriendsPreviewContainer.Children.Add(fGrid);
                }
            }
        }

        private void OpenFriends_Click(object sender, RoutedEventArgs e)
        {
            var friendsView = new FriendsListControl();
            FriendsOverlay.Content = friendsView;
            MainProfileLayout.Visibility = Visibility.Collapsed;
            FriendsOverlay.Visibility = Visibility.Visible;
        }

        private void GoToStore_Internal_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow main)
            {
                main.OpenStore_Click(null, null);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var editControl = new EditProfileControl();
            FriendsOverlay.Content = editControl;

            MainProfileLayout.Visibility = Visibility.Collapsed;
            FriendsOverlay.Visibility = Visibility.Visible;
        }
    }
}