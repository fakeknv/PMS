using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PMS
{
	public class FeeConverter : IValueConverter
	{
		public FeeConverter()
		{

		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null) {
				return "PHP 0.00";
			}
			else {
				return "PHP " + String.Format("{0:0.00}", Double.Parse(value.ToString()));
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
