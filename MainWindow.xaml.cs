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
                AccountMenuButton.Content = Session.CurrentUser.Nickname + " ▼";
                UserBalanceText.Text = "Баланс: " + Session.CurrentUser.Balance.ToString("N2") + " руб.";
            }
            UpdateMenuHighlight(BtnStore);
        }
        private void UpdateMenuHighlight(Button selectedButton)
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
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void MinimizeApp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
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
    }
}