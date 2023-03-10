using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DrawboardPDFApp.Converters
{
    public class BooleanToInvertedVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                switch (boolValue)
                {
                    case true: return Visibility.Collapsed;
                    case false: return Visibility.Visible;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility visibility)
            {
                switch (visibility)
                {
                    case Visibility.Collapsed: return true;
                    case Visibility.Visible: return false;
                }
            }
            return value;
        }
    }
}
