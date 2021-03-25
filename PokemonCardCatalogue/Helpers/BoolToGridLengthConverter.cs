using System;
using System.Globalization;
using Xamarin.Forms;

namespace PokemonCardCatalogue.Helpers
{
    public class BoolToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (parameter is string valueIfTrueString)
                {
                    return BoolToGridLength(boolValue, int.Parse(valueIfTrueString));
                }
            }

            return new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private GridLength BoolToGridLength(bool value, int sizeIfTrue)
        {
            return value ? new GridLength(sizeIfTrue) : new GridLength(0); 
        }
    }
}
