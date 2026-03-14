using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SteamLamp
{
    public partial class MainWindow : Window
    {
        private List<Game> _cartList = new List<Game>();
        private List<Game> _showcaseGames;
        private int _currentPage = 0;
        private HashSet<string> _cartItems = new HashSet<string>();
        public enum UserRole { Guest, User, Admin }
        public UserRole CurrentRole = UserRole.Guest;

        public static class currentUser
        {
            public static string Nickname { get; set; } = "Войти";
            public static string Role { get; set; } = "Guest";
            public static decimal Balance { get; set; } = 0;

            public static bool IsAuthorized => Role != "Guest";
        }
        public MainWindow()
        {
            InitializeComponent();
            UpdateUIState();
            this.Loaded += (s, e) => { MainContentFrame.Content = DefaultStoreView; LoadGamesFromDB(); };

            if (Session.CurrentUser != null)
            {
                AccountMenuButton.Content = Session.CurrentUser.Nickname + " ▼";
                UserBalanceText.Text = "Баланс: " + Session.CurrentUser.Balance.ToString("N2") + " руб.";
                SetOnlineStatus(true);
            }
            UpdateMenuHighlight(BtnStore);
        }
<<<<<<< HEAD
        private void ApplyGuestMode() 
        {
            CurrentRole = UserRole.Guest;
            LoginButton.Visibility = Visibility.Visible;
            AccountMenuButton.Visibility = Visibility.Collapsed;
            CartCountText.Text = "0";
            _cartList.Clear();
        }
        private bool CheckAccess() 
        {
            if (CurrentRole == UserRole.Guest) 
            {
                MessageBox.Show("Для этого действия необходимо войти в аккаунт или зарегистрироваться.","Доступ ограничен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
=======
        private void SetOnlineStatus(bool isOnline)
        {
            try
            {
                if (Session.CurrentUser != null)
                {
                    using (var db = new AppDbContext())
                    {
                        var user = db.Users.Find(Session.CurrentUser.Id);
                        if (user != null)
                        {
                            user.IsOnline = isOnline;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка статуса: " + ex.Message);
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            SetOnlineStatus(false);
            base.OnClosed(e);
        }

>>>>>>> 25c9404e1db1e44901244c77f4b01b1d122a43af
        private void LoadGamesFromDB()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var allGames = db.Games.Where(g => !g.IsShowcase).ToList();
                    GamesListControl.ItemsSource = allGames;
                    _showcaseGames = db.Games.Where(g => g.IsShowcase).ToList();
                    if (_showcaseGames != null && _showcaseGames.Any())
                    {
                        _currentPage = 0;
                        UpdateShowcase();
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Ошибка загрузки Базы: " + error.Message);
            }
        }

        public void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckAccess()) return;
            e.Handled = true;
            var btn = sender as Button;
            Game gameToAdd = btn.DataContext as Game;
            if (gameToAdd == null)
            {
                string title = btn.Tag?.ToString() ?? ModalGameTitle.Text;
                using (var db = new AppDbContext())
                {
                    gameToAdd = db.Games.FirstOrDefault(g => g.Title == title);
                }
            }
            if (gameToAdd != null)
            {
                if (gameToAdd.Price.Trim().Equals("Бесплатно.", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show($"{gameToAdd.Title} успешно добавлена в вашу библиотеку!");
                    OpenLibrary_Click(null, null);
                    return;
                }
                if (!_cartList.Any(g => g.Title == gameToAdd.Title))
                {
                    _cartList.Add(gameToAdd);
                    UpdateCartUI();
                    var blueBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#66c0f4"));
                    var defaultBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3d4450"));
                    BtnCart.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#66c0f4"));
                    var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
                    timer.Tick += (s, args) =>
                    {
                        BtnCart.Background = defaultBrush;
                        timer.Stop();
                    };
                    timer.Start();
                }
            }
        }

        private void UpdateCartUI()
        {
            CartCountText.Text = _cartList.Count.ToString();
        }

        public void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_showcaseGames != null && _currentPage < _showcaseGames.Count - 1)
            {
                _currentPage++;
                UpdateShowcase();
            }
        }

        public void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 0)
            {
                _currentPage--;
                UpdateShowcase();
            }
        }

        public void UpdateShowcase()
        {
            if (_showcaseGames == null || !_showcaseGames.Any()) return;

            var currentGame = _showcaseGames[_currentPage];
            ShowcaseName.Text = currentGame.Title;
            ShowcasePrice.Text = currentGame.Price;
            ShowcaseGameTitle.Text = currentGame.Title;
            ShowcaseBuyBtn.DataContext = currentGame;

            if (PageIndicators != null)
            {
                for (int i = 0; i < PageIndicators.Children.Count; i++)
                {
                    if (PageIndicators.Children[i] is Rectangle rect)
                    {
                        rect.Opacity = (i == _currentPage) ? 1.0 : 0.3;
                    }
                }
            }
        }

        public void UpdateMenuHighlight(Button selectedButton)
        {
            BtnStore.Tag = null;
            BtnLibrary.Tag = null;
            BtnProfile.Tag = null;
            if (selectedButton != null) selectedButton.Tag = "Selected";
        }

        public void OpenStore_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckAccess()) return;
            MainContentFrame.Content = DefaultStoreView;
            UpdateMenuHighlight(BtnStore);
            LoadGamesFromDB();
        }

        public void OpenLibrary_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckAccess()) return;
            MainContentFrame.Content = new LibraryPage();
            UpdateMenuHighlight(BtnLibrary);
        }

        public void OpenProfile_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckAccess()) return;
            ProfilePage profilePage = new ProfilePage();
            MainContentFrame.Content = profilePage;
            UpdateMenuHighlight(BtnProfile);
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            AccountPopup.IsOpen = false;
            ProfilePage profilePage = new ProfilePage();
            MainContentFrame.Content = profilePage;
            UpdateMenuHighlight(BtnProfile);
            EditProfileControl editControl = new EditProfileControl();
            profilePage.FriendsOverlay.Content = editControl;
            profilePage.MainProfileLayout.Visibility = Visibility.Collapsed;
            profilePage.FriendsOverlay.Visibility = Visibility.Visible;
        }

        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e) { this.Close(); } // Изменил на Close(), чтобы сработал OnClosed

        private void MinimizeApp_Click(object sender, RoutedEventArgs e) { this.WindowState = WindowState.Minimized; }

        private void MaximizeApp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = (this.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        }

        private void AccountMenuButton_Click(object sender, RoutedEventArgs e)
        {
            AccountPopup.IsOpen = !AccountPopup.IsOpen;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // ШАГ 3: Сбрасываем статус при выходе из аккаунта (Logout)
            SetOnlineStatus(false);

            AccountPopup.IsOpen = false;
            Session.CurrentUser = null;
            SIGN_UP authWindow = new SIGN_UP();
            authWindow.RegisterForm.Visibility = Visibility.Collapsed;
            authWindow.LoginForm.Visibility = Visibility.Visible;
            authWindow.Show();
            this.Close();
        }

        private void GameCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Game selectedgame)
            {
                OpenGameDetails(selectedgame);
            }
        }

        private void CloseOverlay_Click(object sender, RoutedEventArgs e)
        {
            GameDetailsOverlay.Visibility = Visibility.Collapsed;
        }

        public void OpenCart_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckAccess()) return;
            MainContentFrame.Content = new CartPage(_cartList, this);
            UpdateMenuHighlight(null);
        }

        public void ClearCart()
        {
            _cartList.Clear();
            UpdateCartUI();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = SearchBox.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(search))
            {
                SearchPopup.IsOpen = false;
                return;
            }
            using (var db = new AppDbContext())
            {
                var fillterGames = db.Games.Where(g => g.Title.ToLower().Contains(search)).Take(5).ToList();
                if (fillterGames.Any())
                {
                    SearchResultsList.ItemsSource = fillterGames;
                    SearchPopup.IsOpen = true;
                }
                else
                {
                    SearchPopup.IsOpen = false;
                }
            }
        }
        private void SearchResultsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchResultsList.SelectedItem is Game selectedgame)
            {
                OpenGameDetails(selectedgame);
                SearchPopup.IsOpen = false;
                SearchBox.Text = "";
                SearchResultsList.SelectedItem = null;
            }
        }
        private void OpenGameDetails(Game game)
        {
            if (game == null) return;
            ModalGameTitle.Text = game.Title;
            ModalGameDesc.Text = game.Description;
            ModalGamePrice.Text = game.Price;
            ModalBuyBtn.DataContext = game;
            ModalBuyBtn.Tag = game.Title;
            GameDetailsOverlay.Visibility = Visibility.Visible;
        }
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchPopup.IsOpen = true;
            }
        }
        private void OpenWallet_Click(object sender, RoutedEventArgs e)
        {
            AccountPopup.IsOpen = false;
            MainContentFrame.Content = new WalletPage(this);
        }
        public void UpdateUIState()
        {
            if (Session.CurrentUser != null)
            {
                CurrentRole = UserRole.User; 
                LoginButton.Visibility = Visibility.Collapsed;
                AccountMenuButton.Visibility = Visibility.Visible;
                AccountMenuButton.Content = Session.CurrentUser.Nickname + " ▼";
                UserBalanceText.Text = "Баланс: " + Session.CurrentUser.Balance.ToString("N2") + " руб.";
            }
            else
            {
                ApplyGuestMode();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            SIGN_UP authWindow = new SIGN_UP();
            authWindow.RegisterForm.Visibility = Visibility.Collapsed;
            authWindow.LoginForm.Visibility = Visibility.Visible;
            authWindow.Show();
            this.Close();
        }
    }
}