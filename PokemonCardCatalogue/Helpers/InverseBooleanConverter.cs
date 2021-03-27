using System;
using System.Globalization;
using Xamarin.Forms;

namespace PokemonCardCatalogue.Helpers
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Conversion(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Conversion(value);
        }

        private bool Conversion(object value)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }

            return false;
        }
    }
}
