using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace SteamLamp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (Session.CurrentUser != null)
            {
                AccountMenuButton.Content = $"{Session.CurrentUser.Nickname} ▼";
                UserBalanceText.Text = $"Баланс: {Session.CurrentUser.Balance:N2} руб.";
            }
        }
        public void OpenStore_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Content = DefaultStoreView;
            BtnStore_Focus();
        }
        private void OpenProfile_Click(object sender, RoutedEventArgs e)
        {
            ProfilePage profilePage = new ProfilePage();
            MainContentFrame.Content = profilePage;
            if (sender is Button btn) btn.Focus();
        }
        private void BtnStore_Focus()
        {
            foreach (var child in ((StackPanel)AccountMenuButton.Parent).Children)
            {
                if (child is Button btn && btn.Content.ToString() == "МАГАЗИН")
                {
                    btn.Focus();
                    break;
                }
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

        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void MinimizeApp_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;

        private void MaximizeApp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void AccountMenuButton_Click(object sender, RoutedEventArgs e) => AccountPopup.IsOpen = !AccountPopup.IsOpen;

        private void OpenLibrary_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Content = new LibraryPage();
            BtnLibrary.Focus();
        }
    }
}