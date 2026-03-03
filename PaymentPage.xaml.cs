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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SteamLamp
{
    /// <summary>
    /// Логика взаимодействия для PaymentPage.xaml
    /// </summary>
    public partial class PaymentPage : UserControl
    {
        private List<Game> _items;
        private MainWindow _main;
        private decimal _totalAmount;
        
        public PaymentPage(List<Game> items, MainWindow main)
        {
            InitializeComponent();
            _items = items;
            _main = main;
            _totalAmount = CalculateTotal();
            BalanceInfoText.Text = $"Текущий баланс: {Session.CurrentUser.Balance:N2} руб.";
            FinalTotalText.Text = $"К оплате: {_totalAmount:N2} руб.";
            
        }
        private decimal CalculateTotal() 
        {
            return _items.Sum(i => { string p = i.Price.Replace("руб. ", "").Replace(" ", "").Trim(); return decimal.TryParse(p, out decimal res) ? res : 0m; });
        }

        private void ConfirmPayment_Click(object sender, RoutedEventArgs e)
        {
           
            if (Session.CurrentUser.Balance >= _totalAmount)
            {
                Session.CurrentUser.Balance -= _totalAmount;
                _main.ClearCart();
                _main.OpenStore_Click(null, null);
            }
            else 
            {
                MessageBox.Show("Недостаточно средств");
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _main.OpenCart_Click(null, null);
        }
    }
}
