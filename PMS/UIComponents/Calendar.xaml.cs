using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace PMS.UIComponents
{
    /// <summary>
    /// Interaction logic for Calendar.xaml
    /// </summary>
    public partial class Calendar : UserControl
    {
		public ObservableCollection<Day> Days { get; set; }
		public ObservableCollection<string> DayNames { get; set; }

		public DateTime ActiveDate = DateTime.Today;
		public Calendar()
        {
            InitializeComponent();
			monthNameHolder.Content = DateTime.Today.ToString("MMMM yyyy");

			DayNames = new ObservableCollection<string> { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

			HeaderContainer.ItemsSource = DayNames;

			Days = new ObservableCollection<Day>();
			BuildCalendar(DateTime.Today);
		}

		/// <summary>
		/// Populates the calendar with days.
		/// </summary>
		public void BuildCalendar(DateTime targetDate) {
			Days.Clear();

			DateTime d = new DateTime(targetDate.Year, targetDate.Month, 1);
			int offset = DayOfWeekNumber(d.DayOfWeek);
			//using (offset != 1) will break October 2018
			if (offset != 0)
			{
				d = d.AddDays(-offset);
			}
			//Show 6 weeks each with 7 days = 42
			for (int box = 1; box <= 42; box++)
			{
				Day day = new Day { Date = d, IsTargetMonth = targetDate.Month == d.Month };
				day.IsToday = d == DateTime.Today;
				Days.Add(day);
				d = d.AddDays(1);
			}
			DaysContainer.ItemsSource = Days;
		}
		private static int DayOfWeekNumber(DayOfWeek dow)
		{
			return Convert.ToInt32(dow.ToString("D"));
		}

		/// <summary>
		/// Interaction logic for the Next Month Button.
		/// </summary>
		private void NextMonthButton_Click(object sender, RoutedEventArgs e)
		{
			ActiveDate = ActiveDate.AddMonths(1);
			monthNameHolder.Content = ActiveDate.ToString("MMMM yyyy");
			BuildCalendar(ActiveDate);
		}
		/// <summary>
		/// Interaction logic for the Previous Month Button.
		/// </summary>
		private void PrevMonthButton_Click(object sender, RoutedEventArgs e)
		{
			ActiveDate = ActiveDate.AddMonths(-1);
			monthNameHolder.Content = ActiveDate.ToString("MMMM yyyy");
			BuildCalendar(ActiveDate);
		}
	}
}
