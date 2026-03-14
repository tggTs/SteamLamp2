using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
                if (Session.CurrentUser.Avatar != null)
                {
                    ProfileAvatar.Source = BytesToImage(Session.CurrentUser.Avatar);
                }

                LoadFriendsPreview();
            }
        }
        private BitmapImage BytesToImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;
            try
            {
                var image = new BitmapImage();
                using (var mem = new MemoryStream(bytes))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = mem;
                    image.EndInit();
                }
                image.Freeze();
                return image;
            }
            catch
            {
                return null;
            }
        }

        private void LoadFriendsPreview()
        {
            if (Session.CurrentUser == null) return;
            FriendsPreviewContainer.Children.Clear();
            using (var db = new AppDbContext())
            {
                int myId = Session.CurrentUser.Id;
                var myFriends = (from f in db.Friends
                                 join u in db.Users on f.FriendId equals u.Id
                                 where f.UserId == myId
                                 select new { u.Nickname, u.Avatar }).Take(5).ToList();

                foreach (var friend in myFriends)
                {
                    Grid fGrid = new Grid { Margin = new Thickness(0, 0, 0, 8) };
                    Image friendImg = new Image { Stretch = Stretch.UniformToFill };
                    if (friend.Avatar != null) friendImg.Source = BytesToImage(friend.Avatar);

                    fGrid.Children.Add(new Border
                    {
                        Width = 30,
                        Height = 30,
                        Background = new SolidColorBrush(Color.FromRgb(48, 56, 67)),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        CornerRadius = new CornerRadius(2),
                        Child = friendImg,
                        ClipToBounds = true,
                        BorderBrush = new SolidColorBrush(Color.FromRgb(102, 192, 244)),
                        BorderThickness = new Thickness(1)
                    });
                    fGrid.Children.Add(new TextBlock
                    {
                        Text = friend.Nickname,
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