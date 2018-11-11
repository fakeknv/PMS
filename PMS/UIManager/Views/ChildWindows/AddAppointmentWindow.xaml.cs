using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for DummyWindow.xaml
	/// </summary>
	public partial class AddAppointmentWindow : ChildWindow
	{
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

		public AddAppointmentWindow()
		{
			InitializeComponent();
			MassType.SelectionChanged -= EnableDisableSoul;
			MassType.SelectedIndex = 0;
			SelectedDate1.Text = Appointments.app.Current_Date;
			MassType.SelectionChanged += EnableDisableSoul;
			GetFixedTimeSchedules();
		}

		private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
		private void GetFixedTimeSchedules() {
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM settings WHERE key_name = @key_name;";
				cmd.Parameters.AddWithValue("@key_name", "Fixed Time Schedule Entry");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					SelectedTime1.Items.Add(DateTime.Parse(db_reader.GetString("key_value")).ToString("HH:mm tt"));
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}
		private void EnableDisableSoul(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (MassType.SelectedIndex == 3 || MassType.SelectedIndex == 4) {
				SoulsOf.IsEnabled = true;
			}
			else {
				SoulsOf.IsEnabled = false;
			}
			FetchMassFee();
		}
		/// <summary>
		/// Fetches default confirmation stipend value.
		/// </summary>
		private int FetchMassFee()
		{
			int ret = 0;
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				if (MassType.SelectedIndex == 0) {
					cmd.CommandText = "SELECT key_value FROM settings WHERE key_name = 'Thanksgiving Mass Fee';";
				}
				else if (MassType.SelectedIndex == 1)
				{
					cmd.CommandText = "SELECT key_value FROM settings WHERE key_name = 'Petition Mass Fee';";
				}
				else if (MassType.SelectedIndex == 2)
				{
					cmd.CommandText = "SELECT key_value FROM settings WHERE key_name = 'Special Intention Fee';";
				}
				else if (MassType.SelectedIndex == 3)
				{
					cmd.CommandText = "SELECT key_value FROM settings WHERE key_name = 'All Souls Fee';";
				}
				else if (MassType.SelectedIndex == 4)
				{
					cmd.CommandText = "SELECT key_value FROM settings WHERE key_name = 'Soul/s of Fee';";
				}
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Fee.Value = Convert.ToInt32(db_reader.GetString("key_value"));
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{
				ret = 0;
			}
			return ret;
		}
		private void CreateMassRecord(object sender, RoutedEventArgs e)
		{
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			//TODO
			try
			{
				string apmID = pmsutil.GenAppointmentID();
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText =
					"INSERT INTO appointments(appointment_id, appointment_date, appointment_time, appointment_type, requested_by)" +
					"VALUES(@appointment_id, @appointment_date, @appointment_time, @appointment_type, @requested_by)";
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@appointment_id", apmID);
				cmd.Parameters.AddWithValue("@appointment_date", DateTime.Parse(SelectedDate1.Text).ToString("yyyy-mm-dd"));
				cmd.Parameters.AddWithValue("@appointment_time", DateTime.Parse(SelectedTime1.Text).ToString("HH:mm:ss"));
				cmd.Parameters.AddWithValue("@appointment_type", MassType.Text);
				cmd.Parameters.AddWithValue("@requested_by", OfferedBy1.Text);
				int stat_code = cmd.ExecuteNonQuery();
				dbman.DBClose();
				//string tmp = pmsutil.LogRecord(apmID, "LOGC-01");
			}
			catch (MySqlException ex)
			{
				Console.WriteLine("Error: {0}", ex.ToString());
			}
			this.Close();
		}
	}
}
