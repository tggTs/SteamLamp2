using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SteamLamp
{
    public class PriceToButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string price = value as string;
            if (price != null && price.Trim().Equals("Бесплатно.", StringComparison.OrdinalIgnoreCase))
            {
                return "В библиотеку";
            }
            return "В корзину";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
