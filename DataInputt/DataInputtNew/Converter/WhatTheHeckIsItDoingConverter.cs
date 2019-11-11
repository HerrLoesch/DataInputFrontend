using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DataInputt.Converter
{
    class WhatTheHeckIsItDoingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value.GetType() == typeof(string)) || ((string)value).Length < 1 || (string)value == "" || String.IsNullOrWhiteSpace((string)value))
                return "jetzt";
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
