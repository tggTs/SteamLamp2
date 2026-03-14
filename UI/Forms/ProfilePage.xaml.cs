using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace SteamLamp
{
    public partial class ProfilePage : UserControl
    {
        private readonly SolidColorBrush SteamOnline = new SolidColorBrush(Color.FromRgb(102, 192, 244));
        private readonly SolidColorBrush SteamOffline = new SolidColorBrush(Color.FromRgb(137, 137, 137));

        public ProfilePage()
        {
            InitializeComponent();
            LoadUserData();
        }

        public void LoadUserData(int? userId = null)
        {
            DoubleAnimation fadeOut = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));

            fadeOut.Completed += (s, e) =>
            {
                UpdateProfileData(userId);
                DoubleAnimation fadeIn = new DoubleAnimation(1, TimeSpan.FromSeconds(0.3));
                MainProfileLayout.BeginAnimation(OpacityProperty, fadeIn);
            };

            MainProfileLayout.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void UpdateProfileData(int? userId)
        {
            using (var db = new AppDbContext())
            {
                int targetId = userId ?? Session.CurrentUser.Id;
                var user = db.Users.FirstOrDefault(u => u.Id == targetId);

                if (user != null)
                {
                    ProfileNickname.Text = user.Nickname;
                    ProfileBio.Text = string.IsNullOrEmpty(user.Bio) ? "Здесь будет описание..." : user.Bio;
                    ProfileAvatar.Source = (user.Avatar != null) ? BytesToImage(user.Avatar) : null;
                    if (user.IsOnline)
                    {
                        ProfileNickname.Foreground = SteamOnline;
                        StatusText.Text = "В сети";
                        StatusText.Foreground = SteamOnline;
                    }
                    else
                    {
                        ProfileNickname.Foreground = SteamOffline;
                        StatusText.Text = "Не в сети";
                        StatusText.Foreground = SteamOffline;
                    }
                    if (targetId != Session.CurrentUser.Id)
                    {
                        BtnEditProfile.Visibility = Visibility.Collapsed;
                        BtnBackToMyProfile.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        BtnEditProfile.Visibility = Visibility.Visible;
                        BtnBackToMyProfile.Visibility = Visibility.Collapsed;
                    }

                    LoadFriendsPreview(targetId);
                }
            }
        }

        private void LoadFriendsPreview(int userId)
        {
            FriendsPreviewContainer.Children.Clear();
            using (var db = new AppDbContext())
            {
                var friendsList = (from f in db.Friends
                                   join u in db.Users on f.FriendId equals u.Id
                                   where f.UserId == userId
                                   select new { u.Id, u.Nickname, u.Avatar, u.IsOnline }).Take(5).ToList();

                foreach (var friend in friendsList)
                {
                    Grid fGrid = new Grid { Margin = new Thickness(0, 0, 0, 8), Cursor = Cursors.Hand, Background = Brushes.Transparent };
                    fGrid.MouseLeftButtonDown += (s, e) => LoadUserData(friend.Id);

                    Image img = new Image { Stretch = Stretch.UniformToFill };
                    if (friend.Avatar != null) img.Source = BytesToImage(friend.Avatar);
                    SolidColorBrush friendColor = friend.IsOnline ? SteamOnline : SteamOffline;

                    fGrid.Children.Add(new Border
                    {
                        Width = 32,
                        Height = 32,
                        CornerRadius = new CornerRadius(2),
                        BorderBrush = friendColor,
                        BorderThickness = new Thickness(1),
                        Child = img,
                        ClipToBounds = true,
                        HorizontalAlignment = HorizontalAlignment.Left
                    });

                    fGrid.Children.Add(new TextBlock
                    {
                        Text = friend.Nickname,
                        Foreground = friendColor,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(42, 0, 0, 0),
                        FontSize = 12,
                        FontWeight = FontWeights.SemiBold
                    });

                    FriendsPreviewContainer.Children.Add(fGrid);
                }
            }
        }

        private void BtnBackToMyProfile_Click(object sender, RoutedEventArgs e)
        {
            LoadUserData(null);
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
            catch { return null; }
        }

        private void OpenFriends_Click(object sender, RoutedEventArgs e)
        {
            var friendsView = new FriendsListControl();
            FriendsOverlay.Content = friendsView;
            MainProfileLayout.Visibility = Visibility.Collapsed;
            FriendsOverlay.Visibility = Visibility.Visible;
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