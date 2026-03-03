using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmallGreen.Desktop.Settings.Converters
{
    public class LevelToHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 3 &&
                values[0] is float level &&
                values[1] is float maxLevel &&
                values[2] is double actualHeight)
            {
                if (maxLevel <= 0) return 0d;
                var percentage = Math.Min(level / maxLevel, 1.0);
                // Subtract some padding for the container
                var availableHeight = actualHeight - 60; // Account for header and footer
                return Math.Max(0, availableHeight * percentage);
            }
            return 0d;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
