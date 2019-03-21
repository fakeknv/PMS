using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for DummyWindow.xaml
	/// </summary>
	public partial class MoveAppointmentWindowPopup : ChildWindow
	{
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

		private string _aid;

		public MoveAppointmentWindowPopup(string AptID, DateTime date)
		{
			string sysFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

			_aid = AptID;

			InitializeComponent();
			SelectedDate.Content = date.ToString(sysFormat);

			GetFixedTimeSchedules();

			FetchPriests();

			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM appointments WHERE appointment_id = @aid LIMIT 1;";
				cmd.Parameters.AddWithValue("@aid", AptID);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					FetchMassFee(GetAType(db_reader.GetString("appointment_type")));
					TypeHolder.Content = GetAType(db_reader.GetString("appointment_type"));
					RequestedBy.Text = db_reader.GetString("requested_by");
					Venue.Text = db_reader.GetString("venue");
					Remarks.Text = db_reader.GetString("remarks");

					for (int i = 0; i < SelectedTime.Items.Count; i++)
					{
						if (SelectedTime.Items[i].ToString() == DateTime.Parse(db_reader.GetString("appointment_time")).ToString("hh:mm tt"))
						{
							SelectedTime.SelectedIndex = i;
						}
					}
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{
				
			}
		}
		private void GetFixedTimeSchedules()
		{
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM timeslots WHERE status = 'Active';";
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					SelectedTime.Items.Add(DateTime.Parse(db_reader.GetString("timeslot")).ToString("HH:mm tt"));
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}
		private void FetchPriests()
		{
			AssignedPriest.Items.Clear();

			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM residing_priests WHERE priest_status = 'Active';";
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (IsAvailable(DateTime.Parse(SelectedDate.Content.ToString()).ToString("yyyy-MM-dd"), DateTime.Parse(SelectedTime.Text).ToString("HH:mm:ss"), db_reader.GetString("priest_id")) == false)
					{
						ComboBoxItem ci = new ComboBoxItem();
						ci.IsEnabled = false;
						ci.Content = db_reader.GetString("priest_name");
						AssignedPriest.Items.Add(ci);
					}
					else
					{
						ComboBoxItem ci = new ComboBoxItem();
						ci.IsEnabled = true;
						ci.Content = db_reader.GetString("priest_name");
						AssignedPriest.Items.Add(ci);
					}
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}
		private bool IsAvailable(string adate, string atime, string apriest)
		{
			bool ret = false;
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM appointments WHERE assigned_priest = @apriest AND appointment_date = @adate AND appointment_time = @atime;";
				cmd.Parameters.AddWithValue("apriest", apriest);
				cmd.Parameters.AddWithValue("adate", adate);
				cmd.Parameters.AddWithValue("atime", atime);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (db_reader.GetInt32("COUNT(*)") > 0)
					{
						ret = false;
					}
					else
					{
						ret = true;
					}
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
			return ret;
		}
		/// <summary>
		/// Fetches default confirmation stipend value.
		/// </summary>
		private void FetchMassFee(string type)
		{
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT fee FROM appointment_types WHERE appointment_type = @type;";
				cmd.Parameters.AddWithValue("@type", type);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Fee.Value = db_reader.GetDouble("fee");
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{
				Fee.Value = 0f;
			}
		}
		private bool CheckInputs()
		{
			bool ret = true;

			var bc = new BrushConverter();

			if (string.IsNullOrWhiteSpace(SelectedTime.Text))
			{
				SelectedTime.ToolTip = "This field is required.";
				SelectedTime.BorderBrush = Brushes.Red;
				SelectedTimeIcon.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(AssignedPriest.Text))
			{
				AssignedPriest.ToolTip = "This field is required.";
				AssignedPriest.BorderBrush = Brushes.Red;
				AssignedPriestIcon.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Venue.Text))
			{
				Venue.ToolTip = "This field is required.";
				Venue.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(RequestedBy.Text))
			{
				RequestedBy.ToolTip = "This field is required.";
				RequestedBy.BorderBrush = Brushes.Red;
				RequestedByIcon.BorderBrush = Brushes.Red;

				ret = false;
			}

			return ret;
		}
		private string GetPriest(string name)
		{
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
		private string GetATypeID(string type)
		{
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
		private string GetAType(string type)
		{
			string ret = "";
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT appointment_type FROM appointment_types WHERE type_id = @tid;";
				cmd.Parameters.AddWithValue("@tid", type);
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
		private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
		private async void MsgNotAvailable(string date, string time, string priest)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Not available!", priest + " is not available on the selected date " + date + " " + time +". Please change accordingly and try again.");
		}
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "The schedule has been moved successfully.");

			// Resync Calendar
			PMS.UIComponents.Calendar cal = new UIComponents.Calendar();
			Appointments.app.CalendarHolder.Children.Clear();
			Appointments.app.CalendarHolder.Children.Add(cal);
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
		private void CreateAppointment_Click(object sender, RoutedEventArgs e)
		{
			if (CheckInputs() == true) {
				string selTime = SelectedTime.Text;

				if (IsAvailable(DateTime.Parse(SelectedDate.Content.ToString()).ToString("yyyy-MM-dd"), DateTime.Parse(selTime).ToString("HH:mm:ss"), GetPriest(AssignedPriest.Text)) == false)
				{
					MsgNotAvailable(DateTime.Parse(SelectedDate.Content.ToString()).ToString("MMM dd, yyyy"), DateTime.Parse(selTime).ToString("HH:mm tt"), AssignedPriest.Text);
					this.Close();
				}
				else
				{
					dbman = new DBConnectionManager();
					pmsutil = new PMSUtil();
					//TODO
					try
					{
						MySqlCommand cmd = dbman.DBConnect().CreateCommand();
						cmd.CommandText =
							"UPDATE appointments SET appointment_date = @appointment_date, appointment_time = @appointment_time, appointment_type = @appointment_type, requested_by = @requested_by, placed_by = @placed_by, remarks = @remarks, status = @status, assigned_priest = @a_priest, venue = @venue WHERE appointment_id = @aid;";
						cmd.Parameters.AddWithValue("@aid", _aid);
						cmd.Parameters.AddWithValue("@appointment_date", DateTime.Parse(SelectedDate.Content.ToString()).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@appointment_time", DateTime.Parse(SelectedTime.Text).ToString("HH:mm:ss"));
						cmd.Parameters.AddWithValue("@appointment_type", GetATypeID(TypeHolder.Content.ToString()));
						cmd.Parameters.AddWithValue("@requested_by", RequestedBy.Text);
						cmd.Parameters.AddWithValue("@placed_by", Application.Current.Resources["uid"].ToString());
						cmd.Parameters.AddWithValue("@remarks", Remarks.Text);
						cmd.Parameters.AddWithValue("@status", 1);
						cmd.Parameters.AddWithValue("@a_priest", GetPriest(AssignedPriest.Text));
						cmd.Parameters.AddWithValue("@venue", Venue.Text);
						cmd.Prepare();

						int stat_code = cmd.ExecuteNonQuery();
						dbman.DBClose();
						if (stat_code > 0)
						{
							//Resync Calendar
							//PMS.UIComponents.Calendar cal = new UIComponents.Calendar();
							//Appointments.app.CalendarHolder.Children.Clear();
							//Appointments.app.CalendarHolder.Children.Add(cal);
							this.Close();
							MsgSuccess();
						}
						else
						{
							MsgFail();
						}
						string tmp = pmsutil.LogScheduling(_aid, "LOGC-02");
					}
					catch (MySqlException ex)
					{
						Console.WriteLine("Error: {0}", ex.ToString());
					}
				}
			}
			else {
				
			}
		}

		private void SelectedTime_DropDownClosed(object sender, EventArgs e)
		{
			FetchPriests();
		}
	}
}
