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
        private List<Game> _showcaseGames;
        private int _currentPage = 0;
        private HashSet<string> _cartItems = new HashSet<string>();
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += (s, e) => { MainContentFrame.Content = DefaultStoreView; LoadGamesFromDB(); };
            if (Session.CurrentUser != null)
            {
                AccountMenuButton.Content = Session.CurrentUser.Nickname + " ▼";
                UserBalanceText.Text = "Баланс: " + Session.CurrentUser.Balance.ToString("N2") + " руб.";
            }
            UpdateMenuHighlight(BtnStore);

        }
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
            e.Handled = true;
            var btn = sender as Button;
            string gameToAdd = btn.Tag?.ToString();
            if (string.IsNullOrEmpty(gameToAdd)) 
            {
                gameToAdd = ModalGameTitle.Text;
            }
            if (!string.IsNullOrEmpty(gameToAdd) && !_cartItems.Contains(gameToAdd)) 
            {
                _cartItems.Add(gameToAdd);
                CartCountText.Text = _cartItems.Count.ToString();
                var blueBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#66c0f4"));
                var defaultBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3d4450"));

                BtnCart.Background = blueBrush;
                var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
                timer.Tick += (s, args) => {
                    BtnCart.Background = defaultBrush;
                    timer.Stop();
                };
                timer.Start();
            }
        }

        public void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_showcaseGames != null && _currentPage < _showcaseGames.Count-1)
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
            if (_showcaseGames ==null || !_showcaseGames.Any())
            {
                return;
            }
            var currentGame = _showcaseGames[_currentPage];
            ShowcaseName.Text = currentGame.Title;
            ShowcasePrice.Text = currentGame.Price;
            ShowcaseGameTitle.Text = currentGame.Title;

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
            selectedButton.Tag = "Selected";
        }

        public void OpenStore_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Content = DefaultStoreView;
            UpdateMenuHighlight(BtnStore);
            LoadGamesFromDB();
        }

        private void OpenLibrary_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Content = new LibraryPage();
            UpdateMenuHighlight(BtnLibrary);
        }

        private void OpenProfile_Click(object sender, RoutedEventArgs e)
        {
            ProfilePage profilePage = new ProfilePage();
            MainContentFrame.Content = profilePage;
            UpdateMenuHighlight(BtnProfile);
        }

        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e) { Application.Current.Shutdown(); }

        private void MinimizeApp_Click(object sender, RoutedEventArgs e) { this.WindowState = WindowState.Minimized; }

        private void MaximizeApp_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {

                this.WindowState = WindowState.Normal;

            }
            else
            {

                this.WindowState = WindowState.Maximized;

            }
        }

        private void AccountMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (AccountPopup.IsOpen == true)
            {

                AccountPopup.IsOpen = false;

            }
            else
            {

                AccountPopup.IsOpen = true;

            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
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
            var btn = sender as Button;
            var game = btn.Tag as Game;
            if (game != null)
            {
                ModalGameTitle.Text = game.Title;
                ModalGameDesc.Text = game.Description;
                ModalGamePrice.Text = game.Price;
                ModalBuyBtn.Tag = game.Title;
                if (game.Price.Trim().Equals("Бесплатно.", StringComparison.OrdinalIgnoreCase))
                {
                    ModalBuyBtn.Content = "Добавить в Библиотеку";
                }
                else 
                {
                    ModalBuyBtn.Content = "В корзину";
                }
                    GameDetailsOverlay.Visibility = Visibility.Visible;
            }
        }
        private void CloseOverlay_Click(object sender, RoutedEventArgs e) 
        {
            GameDetailsOverlay.Visibility = Visibility.Collapsed;
        }
        
    }
    public class PriceToButtonTextConverter : System.Windows.Data.IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string price = value as string;
            if (price != null && price.Trim().Equals("Бесплатно.", StringComparison.OrdinalIgnoreCase))
            {
                return "В БИБЛИОТЕКУ";
            }
            return "В КОРЗИНУ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}