using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PMS
{
	public class StatusConverter : IValueConverter
	{
		public StatusConverter()
		{

		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
				return "";
			}
			else if ((string)value == "Paid" || (string)value == "Finished")
			{
				return "#FF3C7AC9";
			}
			else if ((string)value == "Unpaid" || (string)value == "Ongoing")
			{
				return "#FF53951C";
			}
			else if ((string)value == "Cancelled")
			{
				return "#FFC94F3C";
			}
			else {
				return "";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
