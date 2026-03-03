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
    /// Логика взаимодействия для CartPage.xaml
    /// </summary>
    public partial class CartPage : UserControl
    {
        private List<Game> _items;
        private MainWindow _main;
        public CartPage(List<Game> items, MainWindow main)
        {
            InitializeComponent();
            _items = items;
            _main = main;
            Refresh();
        }
        private void Refresh() 
        {
            CartItemsControl.ItemsSource = null;
            CartItemsControl.ItemsSource = _items;
            double total = _items.Sum(i => { string p = i.Price.Replace("руб.", "").Replace(" ", "").Trim(); return double.TryParse(p, out double res) ? res : 0; });
            TotalAmountText.Text = $"{total:N2} руб.";
        }
        private void RemoveItem_Click(object sender, RoutedEventArgs e) 
        {
            var game = (sender as Button).Tag as Game;
            _items.Remove(game);
            Refresh();
        }

        private void Purchase_Click(object sender, RoutedEventArgs e)
        {
            _main.MainContentFrame.Content = new PaymentPage(_items, _main);
        }
    }
}
