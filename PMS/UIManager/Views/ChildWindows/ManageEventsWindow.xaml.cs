using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Data;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PMS.UIComponents;
using System.Collections.ObjectModel;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRequestWindow.xaml
	/// </summary>
	public partial class ManageEventsWindow : ChildWindow
	{
		//MYSQL Related Stuff
		private MySqlConnection conn;
		DBConnectionManager dbman;

		private PMSUtil pmsutil;
			
		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public ManageEventsWindow(DateTime dt)
		{
			pmsutil = new PMSUtil();
			InitializeComponent();

			SelectedDate.Content = "Selected Date: " + dt.ToString("MMM dd, yyyy");

			SyncEvent(dt);
		}
		private void SyncEvent(DateTime dt) {
			ObservableCollection<EventsItem> events = new ObservableCollection<EventsItem>();

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
						cmd.CommandText = "SELECT * FROM appointments WHERE appointment_date = @sdate;";
						cmd.Parameters.AddWithValue("@sdate", dt.ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							string status = "null";

							while (db_reader.Read())
							{
								if (IsCustom(db_reader.GetString("appointment_type")) == true)
								{
									if (db_reader.GetInt32("status") == 2)
									{
										status = "Finished";
									}
									else
									{
										status = "Unfinished";
									}
								}
								else
								{
									if (DateTime.Parse(db_reader.GetString("appointment_date")) < DateTime.Now)
									{
										status = "Finished";
									}
									else {
										status = "Unfinished";
									}
								}
								events.Add(new EventsItem()
								{
									Date = DateTime.Parse(db_reader.GetString("appointment_date")).ToString("MMM dd, yyyy"),
									Time = DateTime.Parse(db_reader.GetString("appointment_time")).ToString("h:mm tt"),
									Type = GetAType(db_reader.GetString("appointment_type")),
									Status = status,
									Sponsor = db_reader.GetString("requested_by"),
									Info = db_reader.GetString("remarks")
								});
							}
						}
					}
				}
			}
			EventsHolder.ItemsSource = events;
		}
		private string GetAType(string tid) {
			string ret = "";
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT appointment_type FROM appointment_types WHERE type_id = @tid;";
				cmd.Parameters.AddWithValue("@tid", tid);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = db_reader.GetString("appointment_type");
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{
				ret = "";
			}
			return ret;
		}
		private bool IsCustom(string tid) {
			bool ret = false;
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT custom FROM appointment_types WHERE type_id = @tid;";
				cmd.Parameters.AddWithValue("@tid", tid);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (db_reader.GetInt32("custom") == 1)
					{
						ret = false;
					}
					else {
						ret = true;
					}
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{
				ret = false;
			}
			return ret;
		}
		/// <summary>
		/// Interaction logic for the AddRegConfirm button. Gathers and prepares the data
		/// for database insertion.
		/// </summary>
		private async void ConfirmPayment_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddAppointmentWindow());
		}
		/// <summary>
		/// Closes the AddRequestForm Window.
		/// </summary>
		private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
