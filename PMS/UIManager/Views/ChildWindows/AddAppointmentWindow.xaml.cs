using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;

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
			SelectedDate2.Text = Appointments.app.Current_Date;

			MassType.SelectionChanged += EnableDisableSoul;
			SyncTimePicker();
			GetFixedTimeSchedules();
			FetchATypes();
			FetchPriests();
		}
		private void SyncTimePicker()
		{
			for (int i = 1; i < 13; i++) {
				THours.Items.Add(i.ToString("D2"));
			}
			for (int i = 0; i < 61; i++)
			{
				TMinutes.Items.Add(i.ToString("D2"));
			}
		}
		private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
		private void GetFixedTimeSchedules() {
			//DUMMY DATA CHANGE THIS
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
		private void FetchMassFee()
		{
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT fee FROM appointment_types WHERE appointment_type = @type;";
				cmd.Parameters.AddWithValue("@type", MassType.Text);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Fee.Value = db_reader.GetDouble("fee");
					Console.WriteLine(db_reader.GetDouble("fee"));
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{
				Fee.Value = 0f;
			}
		}
		private void FetchPriests() {
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM residing_priests;";
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					AssignedPriest.Items.Add(db_reader.GetString("priest_name"));
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}
		private void FetchATypes() {
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT appointment_type, custom FROM appointment_types;";
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (db_reader.GetInt32("custom") == 1) {
						MassType.Items.Add(db_reader.GetString("appointment_type"));
					}
					else {
						EventServiceType.Items.Add(db_reader.GetString("appointment_type"));
					}
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{
				
			}
		}
		private string GetATypeID(string type) {
			string ret = "";
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT type_id FROM appointment_types WHERE appointment_type = @type;";
				cmd.Parameters.AddWithValue("@type", type);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = db_reader.GetString("type_id");
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
		private void CreateMassRecord(object sender, RoutedEventArgs e)
		{
			if (TabControl1.SelectedIndex == 0)
			{
				dbman = new DBConnectionManager();
				pmsutil = new PMSUtil();
				//TODO
				try
				{
					string apmID = pmsutil.GenAppointmentID();
					MySqlCommand cmd = dbman.DBConnect().CreateCommand();
					string soulsof_tmp = SoulsOf.Text;
					if (MassType.Text == "All Souls" || MassType.Text == "Soul/s of")
					{
						soulsof_tmp = SoulsOf.Text;
					}
					else
					{
						soulsof_tmp = "NA.";
					}
					cmd.CommandText =
						"INSERT INTO appointments(appointment_id, appointment_date, appointment_time, appointment_type, requested_by, placed_by, remarks, status)" +
						"VALUES(@appointment_id, @appointment_date, @appointment_time, @appointment_type, @requested_by, @placed_by, @remarks, @status)";
					cmd.Parameters.AddWithValue("@appointment_id", apmID);
					cmd.Parameters.AddWithValue("@appointment_date", DateTime.Parse(SelectedDate1.Text).ToString("yyyy-MM-dd"));
					cmd.Parameters.AddWithValue("@appointment_time", DateTime.Parse(SelectedTime1.Text).ToString("HH:mm:ss"));
					cmd.Parameters.AddWithValue("@appointment_type", GetATypeID(MassType.Text));
					cmd.Parameters.AddWithValue("@requested_by", OfferedBy1.Text);
					cmd.Parameters.AddWithValue("@placed_by", Application.Current.Resources["uid"].ToString());
					cmd.Parameters.AddWithValue("@remarks", soulsof_tmp);
					cmd.Parameters.AddWithValue("@status", 1);
					cmd.Prepare();
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
			else
			{
				string selTime = THours.Text +":"+ TMinutes.Text + " " + TimeMode.Text;
				dbman = new DBConnectionManager();
				pmsutil = new PMSUtil();
				//TODO
				try
				{
					string apmID = pmsutil.GenAppointmentID();
					MySqlCommand cmd = dbman.DBConnect().CreateCommand();
					cmd.CommandText =
						"INSERT INTO appointments(appointment_id, appointment_date, appointment_time, appointment_type, requested_by, placed_by, remarks, status, assigned_priest)" +
						"VALUES(@appointment_id, @appointment_date, @appointment_time, @appointment_type, @requested_by, @placed_by, @remarks, @status, @a_priest)";
					cmd.Parameters.AddWithValue("@appointment_id", apmID);
					cmd.Parameters.AddWithValue("@appointment_date", DateTime.Parse(SelectedDate2.Text).ToString("yyyy-MM-dd"));
					cmd.Parameters.AddWithValue("@appointment_time", DateTime.Parse(selTime).ToString("HH:mm:ss"));
					cmd.Parameters.AddWithValue("@appointment_type", GetATypeID(EventServiceType.Text));
					cmd.Parameters.AddWithValue("@requested_by", OfferedBy2.Text);
					cmd.Parameters.AddWithValue("@placed_by", Application.Current.Resources["uid"].ToString());
					cmd.Parameters.AddWithValue("@remarks", Remarks.Text);
					cmd.Parameters.AddWithValue("@status", 1);
					cmd.Parameters.AddWithValue("@a_priest", GetPriest(AssignedPriest.Text));
					cmd.Prepare();
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
		private string GetPriest(string name) {
			string ret = "";
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT priest_id FROM residing_priests WHERE priest_name = @name;";
				cmd.Parameters.AddWithValue("@name", name);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = db_reader.GetString("priest_id");
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
		private void UpdateFee(object sender, SelectionChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT fee FROM appointment_types WHERE appointment_type = @type;";
				cmd.Parameters.AddWithValue("@type", EventServiceType.Text);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Fee2.Value = db_reader.GetDouble("fee");
					Console.WriteLine(">> " + db_reader.GetDouble("fee"));
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{
				Fee.Value = 0f;
			}
		}
	}
}
