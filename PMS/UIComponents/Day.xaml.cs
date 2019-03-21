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
using MahApps.Metro.Controls.Dialogs;
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
					this.Background = Brushes.LightBlue;
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
					//this.AppointmentCountHolder.Visibility = Visibility.Visible;
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
				DayHolder.Content = value.ToString("%d");
				DateVal.Content = value.ToString("MMMM dd, yyyy");

				if (CountEvents(Date) > 0)
				{
					List<string> events = GetEvents(value);

					if (events.Count > 0) {
						EventItem1.Visibility = Visibility.Visible;
						EventItem1Val.Content = events[0];
						if (events.Count > 1)
						{
							EventItem2.Visibility = Visibility.Visible;
							EventItem2Val.Content = events[1];
							Console.WriteLine(events[1]);
							if (events.Count > 2)
							{
								EventItem3.Visibility = Visibility.Visible;
								EventItem3Val.Content = events[2];
							}
						}
					}
					if (CountEvents(Date) > 3)
					{
						MoreIndicator.Visibility = Visibility.Visible;
						MoreValHolder.Content = (CountEvents(Date)-3) + " more.";
					}
					else
					{
						MoreIndicator.Visibility = Visibility.Hidden;
					}
				}
			}
		}
		public Day()
        {
            InitializeComponent();
        }
		private async void Day_Click(object sender, RoutedEventArgs args)
		{
			//Changing label dayActivitiesTitle in Appointments class.
			Appointments.app.Current_Date = this.DateVal.Content.ToString();

			ObservableCollection<AppointmentItem> items = new ObservableCollection<AppointmentItem>();

			DateTime dt = Convert.ToDateTime(this.DateVal.Content.ToString());

			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new ManageEventsWindow(dt));
		}
		private List<string> GetEvents(DateTime d)
		{
			List<string> events = new List<string>();

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

						if (Calendar.cal.AppFilter.Text == "All") {
							cmd.CommandText = "SELECT * FROM appointments WHERE appointment_date = @sdate LIMIT 5;";
						}
						else {
							cmd.CommandText = "SELECT * FROM appointments WHERE appointment_date = @sdate AND assigned_priest = @priest LIMIT 5;";
						}
						cmd.Parameters.AddWithValue("@sdate", d.ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@priest", GetPriestID(Calendar.cal.AppFilter.Text));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								events.Add(GetAType(db_reader.GetString("appointment_type")) + " - " + GetPriest(db_reader.GetString("assigned_priest")));
							}
						}
					}
				}
			}
			return events;
		}
		private string GetAType(string tid)
		{
			string ret = "";

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT appointment_type FROM appointment_types WHERE type_id = @tid;";
					cmd.Parameters.AddWithValue("@tid", tid);
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						ret = db_reader.GetString("appointment_type");
					}
				}
				else
				{
					ret = "";
				}
			}

			return ret;
		}
		private string GetPriest(string pid)
		{
			string ret = "";

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT priest_name FROM residing_priests WHERE priest_id = @pid LIMIT 1;";
					cmd.Parameters.AddWithValue("@pid", pid);
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						ret = db_reader.GetString("priest_name");
					}
				}
				else
				{
					ret = "";
				}
			}

			return ret;
		}
		private string GetPriestID(string pname)
		{
			string ret = "";

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT priest_id FROM residing_priests WHERE priest_name = @pname LIMIT 1;";
					cmd.Parameters.AddWithValue("@pname", pname);
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						ret = db_reader.GetString("priest_id");
					}
				}
				else
				{
					ret = "";
				}
			}

			return ret;
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

		private void DropReceiver(object sender, DragEventArgs e)
		{
			// object draggedItem = e.Data.GetData(this.Name);
			Label lbl = e.Data.GetData("myFormat") as Label;
			EventTypeItemDraggable det = lbl.Content as EventTypeItemDraggable;
			if (det.EventName != null)
			{
				TestDropEvent(det.EventName);
			}
			else {
				TestDropEvent2(det.AppID);
			}
		}
		private async void TestDropEvent(string data) {
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddAppointmentWindowPopup(data, Date));
		}
		private async void TestDropEvent2(string data)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new MoveAppointmentWindowPopup(data, Date));
		}
	}
}
