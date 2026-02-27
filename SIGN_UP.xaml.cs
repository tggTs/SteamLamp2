using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


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
            CaptchaText.Text = $"{n1} + {n2} = ";
            CaptchaInput.Text = "";
        }
        private void ShowSteamMessage(string title, string message, bool isError = false)
        {
            NotificationTitle.Text = title;
            NotificationMessage.Text = message;
            NotificationTitle.Foreground = isError ? Brushes.Red : new SolidColorBrush(Color.FromRgb(102, 192, 244));
            NotificationOverlay.Visibility = Visibility.Visible;
        }
        private void CloseNotification_Click(object sender, RoutedEventArgs e)
        {
            NotificationOverlay.Visibility = Visibility.Collapsed;
            if (isRegistrationSuccess) OpenMainWindow();
        }
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RegLogin.Text) || string.IsNullOrWhiteSpace(RegNick.Text) || string.IsNullOrWhiteSpace(RegPass.Password))
            {
                ShowSteamMessage("ОШИБКА", "Пожалуйста, заполните все поля регистрации!", true);
                return;
            }
            if (CaptchaInput.Text != captchaResult.ToString())
            {
                ShowSteamMessage("ОШИБКА", "Капча введена неверно!", true);
                GenerateCaptcha();
                return;
            }

            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    if (db.Users.Any(u => u.Login == RegLogin.Text))
                    {
                        ShowSteamMessage("ОШИБКА", "Этот логин уже занят!", true);
                        return;
                    }

                    User newUser = new User
                    {
                        Login = RegLogin.Text,
                        Nickname = RegNick.Text,
                        Password = RegPass.Password,
                        Balance = 0
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();
                    Session.CurrentUser = newUser;
                    Session.LoginSource = "Registration";

                    isRegistrationSuccess = true;
                    ShowSteamMessage("УСПЕХ", "Аккаунт создан! Добро пожаловать в Steam Lamp.");
                }
            }
            catch (Exception ex)
            {
                ShowSteamMessage("ОШИБКА БАЗЫ", "Не удалось сохранить данные: " + ex.Message, true);
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LogNick.Text) || string.IsNullOrWhiteSpace(LogPass.Password))
            {
                ShowSteamMessage("ОШИБКА", "Введите никнейм и пароль!", true);
                return;
            }
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
                        ShowSteamMessage("ОШИБКА", "Неверный никнейм или пароль!", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowSteamMessage("ОШИБКА СВЯЗИ", "База данных недоступна: " + ex.Message, true);
            }
        }

        private void OpenMainWindow()
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

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