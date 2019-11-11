using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DataInputt.Converter
{
    class BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color? c = (Color?)value as Color?;
            System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
            object x = null;
            if (converter.CanConvertFrom(typeof(Color?)))
            {
                x = converter.ConvertFrom(c);
            }
            return x;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
