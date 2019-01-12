using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PMS
{
	public class DateConverter : IValueConverter
	{
		public DateConverter()
		{

		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null) {
				return "";
			}
			else {
				return DateTime.Parse(value.ToString()).ToString("yyyy-MM-dd");
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
