using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
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
using MySql.Data.MySqlClient;
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
		private bool isMarked;

		private MySqlConnection conn;
		private DBConnectionManager dbman;
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
		public bool IsMarked {
			get { return isMarked;  }
			set {
				isMarked = value;
				if (this.IsMarked == true) {
					DateTime dt = Convert.ToDateTime(this.DateVal.Content.ToString());
					this.AppointmentCountHolder.Visibility = Visibility.Visible;
					if (CountEvents(dt) == 1) {
						EventCount.Content = CountEvents(dt) + " Event";
					}
					else {
						EventCount.Content = CountEvents(dt) + " Events";
					}
				}
			}
		}
		public DateTime Date
		{
			get { return date; }
  			set {
				date = value;
				DayHolder.Content = value.ToString("dd");
				DateVal.Content = value.ToString("MMMM dd, yyyy");
			}
		}
		public Day()
        {
            InitializeComponent();
        }
		
		private async void Day_Click(object sender, RoutedEventArgs args)
		{
			//Changing label dayActivitiesTitle in Appointments class.
			//DateTime curDate = Convert.ToDateTime(this.DateVal.Content.ToString());
			Appointments.app.Current_Date = this.DateVal.Content.ToString();

			ObservableCollection<AppointmentItem> items = new ObservableCollection<AppointmentItem>();

			DateTime dt = Convert.ToDateTime(this.DateVal.Content.ToString());

			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new ManageEventsWindow(dt));
		}
		private int CountEvents(DateTime d)
		{
			int ret = 0;
			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(appointment_id) FROM appointments WHERE appointment_date = @sdate;";
						cmd.Parameters.AddWithValue("@sdate", d.ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								ret = db_reader.GetInt32("COUNT(appointment_id)");
							}
						}
					}
				}
			}
			return ret;
		}
		private void Focused(object sender, RoutedEventArgs e)
		{
			var bc = new BrushConverter();
			DayMainGrid.Background = (Brush)bc.ConvertFrom("#FF03BAFF");

			DayHolder.Foreground = Brushes.White;
			AppointmentCountHolder.Background = Brushes.White;
			EventCount.Foreground = (Brush)bc.ConvertFrom("#FF03BAFF");
		}

		private void UnFocused(object sender, RoutedEventArgs e)
		{
			DayMainGrid.Background = Brushes.Transparent;

			var bc = new BrushConverter();
			DayHolder.Foreground = (Brush)bc.ConvertFrom("#FF888888");
			AppointmentCountHolder.Background = (Brush)bc.ConvertFrom("#FF03BAFF");
			EventCount.Foreground = Brushes.White;
		}
	}
}
