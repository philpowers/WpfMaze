namespace MazeWpf.TypeConverter
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    
    using mazelib;

    [TypeConverter]
    public class DirectionTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(Direction)) {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value is Direction direction) {
                return new Thickness( direction.HasFlag(Direction.West) ? 4 : 0,
                                      direction.HasFlag(Direction.North) ? 4 : 0,
                                      direction.HasFlag(Direction.East) ? 4 : 0,
                                      direction.HasFlag(Direction.South) ? 4 : 0);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }
    }
}