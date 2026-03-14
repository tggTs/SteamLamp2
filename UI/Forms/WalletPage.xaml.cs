using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SteamLamp
{
    /// <summary>
    /// Логика взаимодействия для WalletPage.xaml
    /// </summary>
    public partial class WalletPage : UserControl
    {
        private MainWindow _main;
        private decimal _amountTofill;
        public WalletPage(MainWindow main)
        {
            InitializeComponent();
            _main = main;
        }

        private void SelectAmount_Click(object sender, RoutedEventArgs e)
        {
            _amountTofill = decimal.Parse((sender as Button).Tag.ToString());
            SelectedAmountText.Text = $"Сумма к зачислению: {_amountTofill:N2} руб.";
            AmountSelectionPanel.Visibility = Visibility.Collapsed;
            PaymentDetailsPanel.Visibility = Visibility.Visible;
        }
        private void ProcessPayment_Click(object sender, RoutedEventArgs e)
        {
            string card = CardNumberBox.Text.Replace(" ", "");
            if (card.Length < 16 || !card.All(char.IsDigit))
            {
                MessageBox.Show("Введите корректный номер карты (16 цифр).");
                return;
            }

            PaymentDetailsPanel.Visibility = Visibility.Collapsed;
            ProcessingPanel.Visibility = Visibility.Visible;
            DoubleAnimation anim = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(4));
            anim.Completed += (s, args) =>
            {
                using (var db = new AppDbContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Id == Session.CurrentUser.Id);
                    if (user != null)
                    {
                        user.Balance += _amountTofill;
                        Session.CurrentUser.Balance = user.Balance;
                        db.SaveChanges();
                    }
                }

                _main.UserBalanceText.Text = $"Баланс: {Session.CurrentUser.Balance:N2} руб.";
                StatusText.Text = "Баланс успешно пополнен!";
                var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                timer.Tick += (st, sa) => {
                    timer.Stop();
                    _main.OpenStore_Click(null, null);
                };
                timer.Start();
            };
            WalletProgressBar.BeginAnimation(ProgressBar.ValueProperty, anim);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => _main.OpenStore_Click(null, null);
        private void BackToAmounts_Click(object sender, RoutedEventArgs e)
        {
            PaymentDetailsPanel.Visibility = Visibility.Collapsed;
            AmountSelectionPanel.Visibility = Visibility.Visible;
        }
    }
}
