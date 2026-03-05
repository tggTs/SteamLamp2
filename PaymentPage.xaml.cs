using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Media.Animation;

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
            decimal total = 0;
            foreach (var game in _items)
            {
                if (string.IsNullOrEmpty(game.Price)) continue;

                string priceRaw = Regex.Replace(game.Price, @"[^0-9,.]", "").Replace(".", ",");

                if (decimal.TryParse(priceRaw, out decimal priceValue))
                {
                    total += priceValue;
                }
            }
            return total;
        }

        private void ConfirmPayment_Click(object sender, RoutedEventArgs e)
        {
            if (Session.CurrentUser.Balance >= _totalAmount)
            {

                ButtonsPanel.Visibility = Visibility.Collapsed;
                ProcessingPanel.Visibility = Visibility.Visible;
                StatusText.Text = "Обработка платежа...";

                DoubleAnimation purchaseAnim = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(5));

                purchaseAnim.Completed += (s, args) =>
                {
                    using (var db = new AppDbContext())
                    {
                        var userInDb = db.Users.FirstOrDefault(u => u.Id == Session.CurrentUser.Id);
                        if (userInDb != null)
                        {
                            userInDb.Balance -= _totalAmount;
                            Session.CurrentUser.Balance = userInDb.Balance;
                            db.SaveChanges();
                        }
                    }                 
                    BalanceInfoText.Text = $"Текущий баланс: {Session.CurrentUser.Balance:N2} руб.";
                    if (_main.UserBalanceText != null)
                    {
                        _main.UserBalanceText.Text = $"Баланс: {Session.CurrentUser.Balance:N2} руб.";
                    }
                    StatusText.Text = "Покупка завершена! Возвращение в магазин...";
                    PaymentProgressBar.BeginAnimation(ProgressBar.ValueProperty, null); // Сброс анимации
                    PaymentProgressBar.Value = 0;
                    DoubleAnimation returnAnim = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(3));

                    returnAnim.Completed += (s2, args2) =>
                    {
                        StartFlightAnimation();
                        _main.ClearCart();
                    };
                    PaymentProgressBar.BeginAnimation(ProgressBar.ValueProperty, returnAnim);
                };
                PaymentProgressBar.BeginAnimation(ProgressBar.ValueProperty, purchaseAnim);
            }
            else
            {
                MessageBox.Show("Недостаточно средств на балансе.");
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _main.OpenCart_Click(null, null);
        }

        private void StartFlightAnimation()
        {
            try
            {
               
                Panel container = VisualTreeHelper.GetChild(_main, 0) as Panel;
                if (container == null) return;

                Rectangle flyRect = new Rectangle
                {
                    Width = 25,
                    Height = 25,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#66c0f4")),
                    RadiusX = 5,
                    RadiusY = 5,
                    IsHitTestVisible = false,
                    RenderTransform = new TranslateTransform()
                };

                container.Children.Add(flyRect);

                Point startPos = this.TransformToVisual(_main).Transform(new Point(this.ActualWidth / 2, this.ActualHeight / 2));
                Point endPos = _main.BtnLibrary.TransformToVisual(_main).Transform(new Point(_main.BtnLibrary.ActualWidth / 2, _main.BtnLibrary.ActualHeight / 2));

                TranslateTransform trans = (TranslateTransform)flyRect.RenderTransform;

                DoubleAnimation animX = new DoubleAnimation(startPos.X, endPos.X, TimeSpan.FromMilliseconds(800));
                DoubleAnimation animY = new DoubleAnimation(startPos.Y, endPos.Y, TimeSpan.FromMilliseconds(800));
                DoubleAnimation animFade = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(800));

                animY.Completed += (s, args) =>
                {
                    container.Children.Remove(flyRect);
                    _main.OpenStore_Click(null, null); 
                };

                trans.BeginAnimation(TranslateTransform.XProperty, animX);
                trans.BeginAnimation(TranslateTransform.YProperty, animY);
                flyRect.BeginAnimation(Rectangle.OpacityProperty, animFade);
            }
            catch
            {
                _main.OpenStore_Click(null, null);
            }
        }
    }
}