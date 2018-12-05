namespace MazeWpf.ValueConverter
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using mazelib;

    public class DirectionValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Direction direction) {
                return new Thickness( direction.HasFlag(Direction.West) ? 4 : 0,
                                      direction.HasFlag(Direction.North) ? 4 : 0,
                                      direction.HasFlag(Direction.East) ? 4 : 0,
                                      direction.HasFlag(Direction.South) ? 4 : 0);
            }
            return null;
        }

	    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}