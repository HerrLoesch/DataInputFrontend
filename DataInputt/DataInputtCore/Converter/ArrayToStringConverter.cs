using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DataInputt.Converter
{
    class ArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";
            if(value is string[] arr)
            {
                if (arr.Length < 1)
                {
                    return "";
                }
            }
            else
            {
                return "";
            }

            StringBuilder builder = new StringBuilder();
            foreach(string item in (string[])value)
            {
                builder.Append(item + ", ");
            }
            builder.Remove(builder.Length - 2, 2);
            return builder.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
