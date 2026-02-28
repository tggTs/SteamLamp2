using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SteamLamp
{
    public partial class FriendsListControl : UserControl
    {
        public FriendsListControl()
        {
            InitializeComponent();
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
            using (var db = new AppDbContext())
            {
                int myId = Session.CurrentUser.Id;
                var myFriendsList = db.Friends.Where(f => f.UserId == myId).ToList();
                foreach (var friend in myFriendsList)
                {
                    CreateFriendCard(friend.FriendId, friend.FriendNickname, isNew: false);
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
        private void CreateFriendCard(int id, string nickname, bool isNew)
        {
            Border card = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(30, 35, 43)),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 5),
                BorderBrush = new SolidColorBrush(Color.FromRgb(42, 46, 51)),
                BorderThickness = new Thickness(1)
            };
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) }); // Аватар
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Ник
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Кнопка
            Border ava = new Border { Background = new SolidColorBrush(Color.FromRgb(102, 192, 244)), Width = 40, Height = 40 };
            TextBlock txt = new TextBlock
            {
                Text = nickname,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(15, 0, 0, 0),
                FontSize = 14,
                FontWeight = FontWeights.Bold
            };
            grid.Children.Add(ava);
            grid.Children.Add(txt);
            Grid.SetColumn(txt, 1);
            if (isNew)
            {
                Button addBtn = new Button
                {
                    Content = "Добавить",
                    Width = 90,
                    Height = 25,
                    Background = new SolidColorBrush(Color.FromRgb(103, 160, 19)),
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                    Cursor = Cursors.Hand,
                    Margin = new Thickness(10, 0, 0, 0)
                };
                addBtn.Click += (s, e) => AddToFriendsInDB(id, nickname);

                grid.Children.Add(addBtn);
                Grid.SetColumn(addBtn, 2);
            }
            card.Child = grid;
            FriendsContainer.Children.Add(card);
        }
        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            string searchName = FriendSearchBox.Text;
            FriendsContainer.Children.Clear();

            using (var db = new AppDbContext())
            {
                var foundUser = db.Users.FirstOrDefault(u => u.Nickname == searchName);
                if (foundUser != null)
                {
                    CreateFriendCard(foundUser.Id, foundUser.Nickname, isNew: true);
                }
                else
                {
                    MessageBox.Show("Пользователь не найден");
                }
            }
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
                    var newFriendLink = new Friend
                    {
                        UserId = myId,
                        FriendId = friendId,
                        FriendNickname = friendNickname
                    };

                    db.Friends.Add(newFriendLink);
                    db.SaveChanges();

                    MessageBox.Show($"{friendNickname} теперь в вашем списке друзей!");
                    FriendsContainer.Children.Clear();
                }
                else
                {
                    MessageBox.Show("Этот пользователь уже в вашем списке друзей.");
                }
            }
        }
    }
}