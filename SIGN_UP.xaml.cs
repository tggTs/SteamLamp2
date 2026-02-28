using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SteamLamp
{
    public partial class SIGN_UP : Window
    {
        private int captchaResult;
        private Random rnd = new Random();
        private bool isRegistrationSuccess = false;

        public SIGN_UP()
        {
            InitializeComponent();
            GenerateCaptcha();
        }

        private void GenerateCaptcha()
        {
            int n1 = rnd.Next(1, 15);
            int n2 = rnd.Next(1, 15);
            captchaResult = n1 + n2;
            CaptchaText.Text = n1.ToString() + " + " + n2.ToString() + " = ";
            CaptchaInput.Text = "";
        }

        private void ShowSteamMessage(string title, string message, bool isError = false)
        {
            NotificationTitle.Text = title;
            NotificationMessage.Text = message;
            if (isError == true)
            {
                NotificationTitle.Foreground = Brushes.Red;
            }
            else
            {
                NotificationTitle.Foreground = new SolidColorBrush(Color.FromRgb(102, 192, 244));
            }
            NotificationOverlay.Visibility = Visibility.Visible;
        }

        private void CloseNotification_Click(object sender, RoutedEventArgs e)
        {
            NotificationOverlay.Visibility = Visibility.Collapsed;
            if (isRegistrationSuccess == true)
            {
                OpenMainWindow();
            }
        }
        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            if (RegLogin.Text == "" || RegNick.Text == "" || RegPass.Password == "")
            {
                ShowSteamMessage("ОШИБКА", "Заполните все поля!", true);
                return;
            }
            if (CaptchaInput.Text != captchaResult.ToString())
            {
                ShowSteamMessage("ОШИБКА", "Неверная капча!", true);
                GenerateCaptcha();
                return;
            }
            LoadingOverlay.Visibility = Visibility.Visible;
            await Task.Delay(2000);
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    if (db.Users.Any(u => u.Login == RegLogin.Text))
                    {
                        ShowSteamMessage("ОШИБКА", "Логин занят!", true);
                        LoadingOverlay.Visibility = Visibility.Collapsed;
                        return;
                    }

                    User newUser = new User();
                    newUser.Login = RegLogin.Text;
                    newUser.Nickname = RegNick.Text;
                    newUser.Password = RegPass.Password;
                    newUser.Balance = 0;

                    db.Users.Add(newUser);
                    db.SaveChanges();

                    Session.CurrentUser = newUser;
                    Session.LoginSource = "Registration";
                    isRegistrationSuccess = true;
                    ShowSteamMessage("УСПЕХ", "Вы зарегистрированы!");
                }
            }
            catch (Exception ex)
            {
                ShowSteamMessage("ОШИБКА", "Что-то пошло не так: " + ex.Message, true);
            }

            LoadingOverlay.Visibility = Visibility.Collapsed;
        }
        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            if (LogNick.Text == "" || LogPass.Password == "")
            {
                ShowSteamMessage("ОШИБКА", "Введите данные!", true);
                return;
            }

            LoadingOverlay.Visibility = Visibility.Visible;
            await Task.Delay(1500);
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Nickname == LogNick.Text && u.Password == LogPass.Password);

                    if (user != null)
                    {
                        Session.CurrentUser = user;
                        Session.LoginSource = "Login";
                        OpenMainWindow();
                    }
                    else
                    {
                        ShowSteamMessage("ОШИБКА", "Неверный ник или пароль!", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowSteamMessage("ОШИБКА", "Ошибка базы: " + ex.Message, true);
            }

            LoadingOverlay.Visibility = Visibility.Collapsed;
        }
        private void OpenMainWindow()
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void SwitchToLogin_Click(object sender, MouseButtonEventArgs e)
        {
            RegisterForm.Visibility = Visibility.Collapsed;
            LoginForm.Visibility = Visibility.Visible;
        }
        private void SwitchToRegister_Click(object sender, MouseButtonEventArgs e)
        {
            LoginForm.Visibility = Visibility.Collapsed;
            RegisterForm.Visibility = Visibility.Visible;
            GenerateCaptcha();
        }
    }
}