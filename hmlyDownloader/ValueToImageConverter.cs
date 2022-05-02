using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace hmlyDownloader
{
    internal class ValueToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)(value ?? "") == "downloading")
            {
                return Application.Current.MainWindow.Resources["i_downloading"];
            }
            else if ((string)(value ?? "") == "done")
            {
                return Application.Current.MainWindow.Resources["i_done"];
            }
            else
            {
                return "";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
