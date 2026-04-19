using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using IOPath = System.IO.Path;
using SteamLamp.UI.Forms;

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

        //  Обработчики заголовка и кнопок окна 
        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }
        private void CloseApp_Click(object sender, RoutedEventArgs e) { this.Close(); }
        private void MinimizeApp_Click(object sender, RoutedEventArgs e) { this.WindowState = WindowState.Minimized; }
        private void MaximizeApp_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                if (MaximizeBtn != null) MaximizeBtn.Content = "⬜️";
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                if (MaximizeBtn != null) MaximizeBtn.Content = "❐";
            }
        }

        //  Логика пользователей и прав 
        private void ApplyGuestMode()
        {
            CurrentRole = UserRole.Guest;
            LoginButton.Visibility = Visibility.Visible;
            AccountMenuButton.Visibility = Visibility.Collapsed;
            BtnSupport.Visibility = Visibility.Collapsed;
            CartCountText.Text = "0";
            _cartList.Clear();
        }

        private bool CheckAccess(UserRole requiredRole = UserRole.User)
        {
            if (CurrentRole == UserRole.Guest)
            {
                MessageBox.Show("Войдите в систему для выполнения этого действия.");
                return false;
            }
            if (requiredRole == UserRole.Admin && CurrentRole != UserRole.Admin)
            {
                MessageBox.Show("Нужны права администратора.");
                return false;
            }
            return true;
        }

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
            catch (Exception ex) { Console.WriteLine("Ошибка статуса: " + ex.Message); }
        }

        protected override void OnClosed(EventArgs e)
        {
            SetOnlineStatus(false);
            base.OnClosed(e);
        }

        //  Работа с данными (Магазин / Витрина) 
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
            catch (Exception error) { MessageBox.Show("Ошибка загрузки Базы: " + error.Message); }
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
                if (gameToAdd.Price.Trim().Replace(".", "").Equals("Бесплатно", StringComparison.OrdinalIgnoreCase))
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
                    BtnCart.Background = blueBrush;
                    var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
                    timer.Tick += (st, args) => { BtnCart.Background = defaultBrush; timer.Stop(); };
                    timer.Start();
                }
            }
        }

        private void UpdateCartUI() => CartCountText.Text = _cartList.Count.ToString();

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
            if (ShowcaseContent != null) ShowcaseContent.DataContext = currentGame;
            if (ShowcaseName != null) ShowcaseName.Text = currentGame.Title;
            if (ShowcasePrice != null) ShowcasePrice.Text = currentGame.Price;
            if (ShowcaseBuyBtn != null) ShowcaseBuyBtn.DataContext = currentGame;
        }

        public void UpdateMenuHighlight(Button selectedButton)
        {
            BtnStore.Tag = null;
            BtnLibrary.Tag = null;
            BtnProfile.Tag = null;
            if (BtnSupport != null) BtnSupport.Tag = null;
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

        public void OpenCart_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckAccess()) return;
            MainContentFrame.Content = new CartPage(_cartList, this);
            UpdateMenuHighlight(null);
        }

        public void OpenWallet_Click(object sender, RoutedEventArgs e)
        {
            AccountPopup.IsOpen = false;
            MainContentFrame.Content = new WalletPage(this);
        }

        public void OpenSupport_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckAccess()) return;
            SendMessageSupport supportPage = new SendMessageSupport();
            MainContentFrame.Content = supportPage;
            UpdateMenuHighlight(BtnSupport);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = SearchBox.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(search))
            {
                SearchPopup.IsOpen = false;
                SearchResultsList.ItemsSource = null;
                return;
            }
            using (var db = new AppDbContext())
            {
                var filtered = db.Games.Where(g => g.Title.ToLower().Contains(search)).Take(5).ToList();
                if (filtered.Any())
                {
                    SearchResultsList.ItemsSource = null;
                    SearchResultsList.ItemsSource = filtered;
                    SearchPopup.IsOpen = true;
                }
                else SearchPopup.IsOpen = false;
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

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchBox.Text)) SearchPopup.IsOpen = true;
        }

        private void OpenGameDetails(Game game)
        {
            if (game == null) return;
            GameDetailsOverlay.DataContext = game;
            ModalGameTitle.Text = game.Title;
            ModalGameDesc.Text = game.Description;
            ModalGamePrice.Text = game.Price;
            ModalAddToCartBtn.Content = game.Price.Trim().Equals("Бесплатно", StringComparison.OrdinalIgnoreCase) ? "Добавить в библиотеку" : "В корзину";
            GameDetailsOverlay.Visibility = Visibility.Visible;
            ModalGameDeveloper.Text = !string.IsNullOrEmpty(game.Developer) ? $"от создателей {game.Developer}" : "";
            ModalGameDeveloper.Visibility = !string.IsNullOrEmpty(game.Developer) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void GameCard_Click(object sender, RoutedEventArgs e)
        {
            var border = sender as FrameworkElement;
            if (border?.Tag is Game game)
            {
                OpenGameDetails(game); 
            }
        }

        private void CloseOverlay_Click(object sender, RoutedEventArgs e) => GameDetailsOverlay.Visibility = Visibility.Collapsed;
        private void AccountMenuButton_Click(object sender, RoutedEventArgs e) => AccountPopup.IsOpen = !AccountPopup.IsOpen;

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            SetOnlineStatus(false);
            AccountPopup.IsOpen = false;
            Session.CurrentUser = null;
            SIGN_UP authWindow = new SIGN_UP();
            authWindow.RegisterForm.Visibility = Visibility.Collapsed;
            authWindow.LoginForm.Visibility = Visibility.Visible;
            authWindow.Show();
            this.Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            SIGN_UP authWindow = new SIGN_UP();
            authWindow.RegisterForm.Visibility = Visibility.Collapsed;
            authWindow.LoginForm.Visibility = Visibility.Visible;
            authWindow.Show();
            this.Close();
        }

        public void UpdateUIState()
        {
            if (Session.CurrentUser != null)
            {
                CurrentRole = Session.CurrentUser.Role == "Admin" ? UserRole.Admin : UserRole.User;
                LoginButton.Visibility = Visibility.Collapsed;
                AccountMenuButton.Visibility = Visibility.Visible;
                BtnSupport.Visibility = Visibility.Visible;
                AccountMenuButton.Content = Session.CurrentUser.Nickname + " ▼";
                ApplyInterfaceByRole(CurrentRole == UserRole.Admin);
                UserBalanceText.Text = "Баланс: " + Session.CurrentUser.Balance.ToString("N2") + " руб.";
            }
            else ApplyGuestMode();
        }

        private void ApplyInterfaceByRole(bool isAdmin)
        {
            if (isAdmin)
            {
                BtnStore.Visibility = Visibility.Collapsed;
                BtnLibrary.Visibility = Visibility.Collapsed;
                BtnRequests.Visibility = Visibility.Visible;
                BtnProfile.Visibility = Visibility.Collapsed;
                BtnUserData.Visibility = Visibility.Visible;
            }
            else
            {
                BtnProfile.Visibility = Visibility.Visible;
                BtnLibrary.Visibility = Visibility.Visible;
                BtnStore.Visibility = Visibility.Visible;
                BtnUserData.Visibility = Visibility.Collapsed;
            }
        }

        public void OpenUserData_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckAccess(UserRole.Admin)) return;
            try
            {
                MainContentFrame.Content = new UI.Forms.AdminDataPage();
                UpdateMenuHighlight(BtnUserData);
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка навигации: {ex.Message}"); }
        }

        public void OpenRequests_Click(object sender, RoutedEventArgs e) 
        {
            if (!CheckAccess(UserRole.Admin))
            {
                return;
            }
            //MainContentFrame.Content = new AdminRequestsPage();
            UpdateMenuHighlight(BtnRequests);
        }
        public void ClearCart() { _cartList.Clear(); UpdateCartUI(); }

        private void BtnAddMyGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addGameWin = new SteamLamp.UI.Forms.AddGameWindow();
                addGameWin.Owner = this;
                addGameWin.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть окно добавления игры: {ex.Message}");
            }
        }
    }


    public class PathToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value as string;
            if (string.IsNullOrEmpty(path)) return null;

            try
            {
                string fullPath = path;

                if (!IOPath.IsPathRooted(path))
                {
                    fullPath = IOPath.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                }

                if (File.Exists(fullPath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(fullPath);
                    bitmap.EndInit();
                    return bitmap;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки картинки: {ex.Message}");
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}