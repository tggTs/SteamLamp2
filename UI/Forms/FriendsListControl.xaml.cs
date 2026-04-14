using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SteamLamp
{
    public partial class FriendsListControl : UserControl
    {
        public FriendsListControl()
        {
            InitializeComponent();

            if (Session.CurrentUser != null)
            {
                FriendsNickName.Text = Session.CurrentUser.Nickname;
                if (Session.CurrentUser.Avatar != null && Session.CurrentUser.Avatar.Length > 0)
                {
                    UserSelfAvatar.Source = BytesToImage(Session.CurrentUser.Avatar);
                }

                LoadMyFriends();
                BtnShowFriends.Background = new SolidColorBrush(Color.FromRgb(61, 68, 80));
            }
        }

        private void BtnShowFriends_Click(object sender, RoutedEventArgs e)
        {
            TabTitle.Text = "ВСЕ ДРУЗЬЯ";
            SearchArea.Visibility = Visibility.Collapsed;
            FriendsContainer.Children.Clear();

            if (Session.CurrentUser != null)
            {
                LoadMyFriends();
            }

            BtnShowFriends.Background = new SolidColorBrush(Color.FromRgb(61, 68, 80));
            BtnAddFriends.Background = Brushes.Transparent;
        }

        private void LoadMyFriends()
        {
            FriendsContainer.Children.Clear();
            using (var db = new AppDbContext())
            {
                int myId = Session.CurrentUser.Id;

                var myFriendsList = (from f in db.Friends join u in db.Users on f.FriendId equals u.Id where f.UserId == myId select new { u.Id, u.Nickname, u.Avatar}).ToList();

                foreach (var friend in myFriendsList)
                {
                    CreateFriendCard(friend.Id, friend.Nickname, isNew: false, avatarBytes: friend.Avatar);
                }
            }
        }

        private void OpenAddFriendTab_Click(object sender, RoutedEventArgs e)
        {
            TabTitle.Text = "ДОБАВИТЬ В ДРУЗЬЯ";
            SearchArea.Visibility = Visibility.Visible;
            FriendsContainer.Children.Clear();

            BtnAddFriends.Background = new SolidColorBrush(Color.FromRgb(61, 68, 80));
            BtnShowFriends.Background = Brushes.Transparent;
        }

        private void CreateFriendCard(int id, string nickname, bool isNew, byte[] avatarBytes = null)
        {
            Border card = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(22, 32, 45)),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 6),
                BorderBrush = new SolidColorBrush(Color.FromRgb(35, 43, 54)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(3),
                Tag = id,
                Cursor = Cursors.Hand
            };

            card.MouseEnter += (s, e) => card.Background = new SolidColorBrush(Color.FromRgb(38, 51, 69));
            card.MouseLeave += (s, e) => card.Background = new SolidColorBrush(Color.FromRgb(22, 32, 45));
            card.MouseLeftButtonDown += (s, e) => NavigateToUserProfile(id);

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(45) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            Image avaImage = new Image { Stretch = Stretch.UniformToFill, Source = avatarBytes != null ? BytesToImage(avatarBytes) : null };
            Border ava = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(48, 56, 67)),
                Width = 40,
                Height = 40,
                CornerRadius = new CornerRadius(2),
                BorderBrush = new SolidColorBrush(Color.FromRgb(102, 192, 244)),
                BorderThickness = new Thickness(1),
                Child = avaImage,
                ClipToBounds = true
            };

            TextBlock txt = new TextBlock
            {
                Text = nickname,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(15, 0, 0, 0),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold
            };

            grid.Children.Add(ava);
            grid.Children.Add(txt);
            Grid.SetColumn(txt, 1);

            if (isNew)
            {
                if (Session.CurrentUser != null && id == Session.CurrentUser.Id)
                {
                    var itsYou = new TextBlock { Text = "ЭТО ВЫ", Foreground = Brushes.Gray, FontSize = 10, FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 10, 0) };
                    grid.Children.Add(itsYou);
                    Grid.SetColumn(itsYou, 2);
                }
                else
                {
                    Button addBtn = new Button { Content = "Добавить", Width = 90, Height = 26, Background = new SolidColorBrush(Color.FromRgb(103, 160, 19)), Foreground = Brushes.White, FontSize = 11, FontWeight = FontWeights.Bold, BorderThickness = new Thickness(0), Cursor = Cursors.Hand };
                    addBtn.Click += (s, e) => {
                        e.Handled = true;
                        AddToFriendsInDB(id, nickname);
                    };
                    grid.Children.Add(addBtn);
                    Grid.SetColumn(addBtn, 2);
                }
            }
            else
            {
                StackPanel actionPanel = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
                TextBlock statusTxt = new TextBlock { Text = "В сети", Foreground = new SolidColorBrush(Color.FromRgb(102, 192, 244)), FontSize = 11, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 10, 0) };
                Button optionsBtn = new Button
                {
                    Content = "● ● ●",
                    FontSize = 7,
                    Width = 32,
                    Height = 22,
                    Cursor = Cursors.Hand,
                    Margin = new Thickness(0, 0, 5, 0)
                };
                if (this.TryFindResource("SteamOptionButtonStyle") is Style buttonStyle)
                {
                    optionsBtn.Style = buttonStyle;
                }
                ContextMenu menu = new ContextMenu();
                if (this.TryFindResource(typeof(ContextMenu)) is Style menuStyle)
                {
                    menu.Style = menuStyle;
                }

                MenuItem removeFriend = new MenuItem { Header = "Удалить из друзей" };
                removeFriend.Click += (s, e) => RemoveFriendFromDB(id, nickname);
                menu.Items.Add(removeFriend);

                optionsBtn.Click += (s, e) => {
                    e.Handled = true;
                    menu.PlacementTarget = optionsBtn;
                    menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                    menu.IsOpen = true;
                };

                actionPanel.Children.Add(statusTxt);
                actionPanel.Children.Add(optionsBtn);

                grid.Children.Add(actionPanel);
                Grid.SetColumn(actionPanel, 2);
            }

            card.Child = grid;
            FriendsContainer.Children.Add(card);
        }
        private void NavigateToUserProfile(int userId)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(this);
            while (parent != null && !(parent is ProfilePage))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            if (parent is ProfilePage profilePage)
            {
                profilePage.FriendsOverlay.Visibility = Visibility.Collapsed;
                profilePage.MainProfileLayout.Visibility = Visibility.Visible;
                profilePage.LoadUserData(userId);
            }
        }

        private void RemoveFriendFromDB(int friendId, string friendNickname)
        {
            var result = MessageBox.Show($"Вы уверены, что хотите удалить {friendNickname} из списка друзей?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes && Session.CurrentUser != null)
            {
                using (var db = new AppDbContext())
                {
                    int myId = Session.CurrentUser.Id;
                    var friendship = db.Friends.Where(f => (f.UserId == myId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == myId)).ToList();

                    if (friendship.Any())
                    {
                        db.Friends.RemoveRange(friendship);
                        db.SaveChanges();
                        LoadMyFriends();
                    }
                }
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            string searchName = FriendSearchBox.Text.Trim();
            if (string.IsNullOrEmpty(searchName) || searchName == "Введите никнейм друга для поиска") return;

            FriendsContainer.Children.Clear();

            using (var db = new AppDbContext())
            {
                var foundUser = db.Users.FirstOrDefault(u => u.Nickname == searchName);
                if (foundUser != null)
                {
                    CreateFriendCard(foundUser.Id, foundUser.Nickname, isNew: true, avatarBytes: foundUser.Avatar);
                }
                else
                {
                    MessageBox.Show("Пользователь не найден");
                }
            }
        }

        private void AddToFriendsInDB(int friendId, string friendNickname)
        {
            if (Session.CurrentUser == null) return;
            int myId = Session.CurrentUser.Id;

            using (var db = new AppDbContext())
            {
                if (myId == friendId)
                {
                    MessageBox.Show("Вы не можете добавить самого себя.");
                    return;
                }

                bool alreadyFriends = db.Friends.Any(f => f.UserId == myId && f.FriendId == friendId);
                if (!alreadyFriends)
                {
                    db.Friends.Add(new Friend
                    {
                        UserId = myId,
                        FriendId = friendId,
                        FriendNickname = friendNickname
                    });
                    db.Friends.Add(new Friend
                    {
                        UserId = friendId,
                        FriendId = myId,
                        FriendNickname = Session.CurrentUser.Nickname
                    });
                    db.SaveChanges();
                    MessageBox.Show($"{friendNickname} теперь в вашем списке друзей!");
                    BtnShowFriends_Click(null, null);
                }
                else
                {
                    MessageBox.Show("Этот пользователь уже в вашем списке друзей.");
                }
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
            catch { return null; }
        }

        private void FriendSearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (FriendSearchBox.Text == "Введите никнейм друга для поиска")
            {
                FriendSearchBox.Text = "";
            }
        }

        private void BackToProfile_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(this);
            while (parent != null && !(parent is ProfilePage))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            if (parent is ProfilePage profilePage)
            {
                profilePage.FriendsOverlay.Visibility = Visibility.Collapsed;
                profilePage.MainProfileLayout.Visibility = Visibility.Visible;
                profilePage.LoadUserData();
            }
        }
    }
}