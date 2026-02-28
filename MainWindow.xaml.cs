using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SteamLamp
{
    public partial class MainWindow : Window
    {
        private int _currentPage = 0;
        private const int TotalPages = 5;
        private HashSet<string> _cartItems = new HashSet<string>();
        private string[] _gameTitles = { "Teardown", "Cities: Skylines II", "Satisfactory", "Upload Labs", "BeamnNG.drive" };
        private string[] _gamePrices = { "1999 руб.", "3049 руб.", "2099 руб.", "Бесплатно", "849 руб." };

        public MainWindow()
        {
            InitializeComponent();

            if (Session.CurrentUser != null)
            {
                AccountMenuButton.Content = Session.CurrentUser.Nickname + " ▼";
                UserBalanceText.Text = "Баланс: " + Session.CurrentUser.Balance.ToString("N2") + " руб.";
            }
            UpdateMenuHighlight(BtnStore);

        }

        public void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            string gameToAdd = "";
            if (_currentPage >= 0 && _currentPage < _gameTitles.Length) 
            {
                var btn = sender as Button;
                var parent = btn.Parent as StackPanel;
                if (parent != null) 
                {
                    foreach (var chils in parent.Children) 
                    {
                        if (chils is TextBlock tb && (tb.Name == "ShowcaseName" || tb.Margin.Top == 5)) 
                        {
                            gameToAdd += tb.Text;
                            break;
                        }
                    }
                }
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
            if (_currentPage < TotalPages - 1)
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
            ShowcaseName.Text = _gameTitles[_currentPage];
            ShowcasePrice.Text = _gamePrices[_currentPage];
            ShowcaseGameTitle.Text = _gameTitles[_currentPage];

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
    }
}