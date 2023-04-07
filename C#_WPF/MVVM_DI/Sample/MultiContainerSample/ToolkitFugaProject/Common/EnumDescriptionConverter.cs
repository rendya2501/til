using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace ToolkitFugaProject.Common;

public class EnumDescriptionConverter : IValueConverter
{
    private static string GetEnumDescription(Enum enumObj)
    {
        foreach (var att in enumObj.GetType().GetField(enumObj.ToString()).GetCustomAttributes(false))
        {
            return att is DescriptionAttribute attrib ? attrib.Description : enumObj.ToString();
        }
        return enumObj.ToString();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Enum myEnum ? GetEnumDescription(myEnum) : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}