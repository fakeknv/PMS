using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.SimpleChildWindow;
using PMS.UIManager.Views;
using PMS.UIManager.Views.ChildWindows;

namespace PMS.UIComponents
{
    /// <summary>
    /// Interaction logic for Day.xaml
    /// </summary>
    public partial class Day : UserControl
    {
		#pragma warning disable 0649
		private DateTime date;
		#pragma warning restore 0649
		private bool isTargetMonth;
		private bool isToday;

		/// <summary>
		/// Checks if the day is the current day.
		/// </summary>
		public bool IsToday
		{
			get { return isToday; }
			set
			{
				isToday = value;
				if (this.IsToday == true)
				{
					this.Background = Brushes.LightGray;
					//If via Hex
					//this.Background = Color.FromArgb(0xFF0­0FF);
				}
			}
		}

		public bool IsTargetMonth
		{
			get { return isTargetMonth; }
			set
			{
				isTargetMonth = value;
			}
		}

		public DateTime Date
		{
			get { return date; }
  			set {
				DayHolder.Content = value.ToString("dd");
				DateVal.Content = value.ToString("MMMM dd, yyyy");
			}
		}
		public Day()
        {
            InitializeComponent();
        }

		private void Day_Click(object sender, RoutedEventArgs args)
		{
			//Changing label dayActivitiesTitle in Appointments class.
			//DateTime curDate = Convert.ToDateTime(this.DateVal.Content.ToString());
			Appointments.app.Current_Date = this.DateVal.Content.ToString();
		}
	}
}
