using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for DummyWindow.xaml
	/// </summary>
	public partial class EditAppointmentWindow : ChildWindow
	{
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

		private ManageEventsWindow _caller;
		private string _aid;

		public EditAppointmentWindow(ManageEventsWindow caller, string aid)
		{
			_aid = aid;
			_caller = caller;
			InitializeComponent();
			MassType.SelectedIndex = 0;
			SelectedDate1.Text = Appointments.app.Current_Date;
			SelectedDate2.Text = Appointments.app.Current_Date;

			SyncTimePicker();
			GetFixedTimeSchedules();
			FetchATypes();
			FetchMassFee();
			UpdateFee2();
			FetchPriests();
			PopulateFields(aid);
			

			if (DateTime.Parse(SelectedDate2.Text) < DateTime.Now)
			{
				SelDateValidator2.Visibility = Visibility.Visible;
				SelDateValidator2.ToolTip = "Notice: Selected date is already over!";
				SelDateValidator2.Foreground = Brushes.Orange;
				SelectedDate2.BorderBrush = Brushes.Orange;
			}
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
		private bool IsCustom(string aid)
		{
			bool ret = false;
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT custom FROM appointments, appointment_types WHERE appointments.appointment_id = @aid AND appointments.appointment_type = appointment_types.type_id;";
				cmd.Parameters.AddWithValue("@aid", aid);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (db_reader.GetInt32("custom") == 1)
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
				ret = false;
			}
			return ret;
		}
		private void PopulateFields(string aid)
		{
			//DUMMY DATA CHANGE THIS
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM appointments WHERE appointment_id = @aid LIMIT 1;";
				cmd.Parameters.AddWithValue("@aid", aid);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (IsCustom(aid) == false)
					{
						Tab1.IsEnabled = true;
						Tab2.IsEnabled = false;
						TabControl1.SelectedIndex = 0;

						for (int i = 0; i < MassType.Items.Count; i++)
						{
							var item = (string)MassType.Items[i];
							if (db_reader.GetString("appointment_type") == GetATypeID(item))
							{
								MassType.SelectedIndex = i;
							}
						}
						SoulsOf.Text = db_reader.GetString("remarks");
						for (int i = 0; i < AssignedPriest.Items.Count; i++)
						{
							var item = (ComboBoxItem)AssignedPriest.Items[i];
							if (db_reader.GetString("assigned_priest") == GetPriest(item.Content.ToString()))
							{
								AssignedPriest.SelectedIndex = i;
							}
						}

						OfferedBy1.Text = db_reader.GetString("requested_by");

						for (int i = 0; i < SelectedTime1.Items.Count; i++)
						{
							var item = (string)SelectedTime1.Items[i];
							if (DateTime.Parse(db_reader.GetString("appointment_time")).ToString("hh:mm tt") == item)
							{
								SelectedTime1.SelectedIndex = i;
							}
						}
					}
					else
					{
						Tab1.IsEnabled = false;
						Tab2.IsEnabled = true;
						TabControl1.SelectedIndex = 1;
						
						for (int i = 0; i < AssignedPriest.Items.Count; i++)
						{
							var item = (ComboBoxItem)AssignedPriest.Items[i];
							if (db_reader.GetString("assigned_priest") == GetPriest(item.Content.ToString()))
							{
								AssignedPriest.SelectedIndex = i;
							}
						}
						for (int i = 0; i < EventServiceType.Items.Count; i++)
						{
							var item = (string)EventServiceType.Items[i];
							if (db_reader.GetString("appointment_type") == GetATypeID(item))
							{
								EventServiceType.SelectedIndex = i;
							}
						}
						Remarks.Text = db_reader.GetString("remarks");
						OfferedBy2.Text = db_reader.GetString("requested_by");
						for (int i = 0; i < THours.Items.Count; i++)
						{
							var item = (string)THours.Items[i];
							if (DateTime.Parse(db_reader.GetString("appointment_time")).ToString("hh") == item)
							{
								THours.SelectedIndex = i;
							}
						}
						for (int i = 0; i < TMinutes.Items.Count; i++)
						{
							var item = (string)TMinutes.Items[i];
							if (DateTime.Parse(db_reader.GetString("appointment_time")).ToString("mm") == item)
							{
								TMinutes.SelectedIndex = i;
							}
						}
						for (int i = 0; i < TimeMode.Items.Count; i++)
						{
							var item = (ComboBoxItem)TimeMode.Items[i];
							if (DateTime.Parse(db_reader.GetString("appointment_time")).ToString("tt") == item.Content.ToString())
							{
								TimeMode.SelectedIndex = i;
							}
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
		private void GetFixedTimeSchedules() {
			//DUMMY DATA CHANGE THIS
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM timeslots WHERE status = 'Active';";
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					SelectedTime1.Items.Add(DateTime.Parse(db_reader.GetString("timeslot")).ToString("HH:mm tt"));
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
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
					string selTime = THours.Text + ":" + TMinutes.Text + " " + TimeMode.Text;

					if (IsAvailable(DateTime.Parse(SelectedDate2.Text).ToString("yyyy-MM-dd"), DateTime.Parse(selTime).ToString("HH:mm:ss"), db_reader.GetString("priest_id")) == false)
					{
						ComboBoxItem ci = new ComboBoxItem();
						ci.IsEnabled = false;
						ci.Content = db_reader.GetString("priest_name");
						AssignedPriest.Items.Add(ci);
					}
					else {
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
		private bool IsAvailable(string adate, string atime, string apriest)
		{
			bool ret = false;
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM appointments WHERE appointment_id != @aid AND assigned_priest = @apriest AND appointment_date = @adate AND appointment_time = @atime;";
				cmd.Parameters.AddWithValue("@aid", _aid);
				cmd.Parameters.AddWithValue("@apriest", apriest);
				cmd.Parameters.AddWithValue("@adate", adate);
				cmd.Parameters.AddWithValue("@atime", atime);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (db_reader.GetInt32("COUNT(*)") > 0) {
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

			}
			return ret;
		}
		private bool CheckInputs()
		{
			var bc = new BrushConverter();

			MassTypeValidator.Visibility = Visibility.Hidden;
			MassTypeValidator.Foreground = Brushes.Transparent;
			MassType.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			SponsorValidator.Visibility = Visibility.Hidden;
			SponsorValidator.Foreground = Brushes.Transparent;
			OfferedBy1.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			SelDateValidator1.Visibility = Visibility.Hidden;
			SelDateValidator1.Foreground = Brushes.Transparent;
			SelectedDate1.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			FeeValidator1.Visibility = Visibility.Hidden;
			FeeValidator1.Foreground = Brushes.Transparent;
			Fee.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			SoulsofValidator.Visibility = Visibility.Hidden;
			SoulsofValidator.Foreground = Brushes.Transparent;
			SoulsOf.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			SponsorValidator2.Visibility = Visibility.Hidden;
			SponsorValidator2.Foreground = Brushes.Transparent;
			OfferedBy2.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			bool ret = true;

			if (string.IsNullOrWhiteSpace(MassType.Text))
			{
				MassTypeValidator.Visibility = Visibility.Visible;
				MassTypeValidator.ToolTip = "This field is required.";
				MassTypeValidator.Foreground = Brushes.Red;
				Fee.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (TabControl1.SelectedIndex == 0) {
				if (string.IsNullOrWhiteSpace(OfferedBy1.Text))
				{
					SponsorValidator.Visibility = Visibility.Visible;
					SponsorValidator.ToolTip = "This field is required.";
					SponsorValidator.Foreground = Brushes.Red;
					OfferedBy1.BorderBrush = Brushes.Red;

					ret = false;
				}
			}
			else {
				if (string.IsNullOrWhiteSpace(OfferedBy2.Text))
				{
					SponsorValidator2.Visibility = Visibility.Visible;
					SponsorValidator2.ToolTip = "This field is required.";
					SponsorValidator2.Foreground = Brushes.Red;
					OfferedBy2.BorderBrush = Brushes.Red;

					ret = false;
				}
			}
			if (string.IsNullOrWhiteSpace(SelectedDate1.Text))
			{
				SelDateValidator1.Visibility = Visibility.Visible;
				SelDateValidator1.ToolTip = "This field is required.";
				SelDateValidator1.Foreground = Brushes.Red;
				SelectedDate1.BorderBrush = Brushes.Red;

				ret = false;
			}
			//FIELD LEVEL VALIDATION
			if (Fee.Value < 0)
			{
				FeeValidator1.Visibility = Visibility.Visible;
				FeeValidator1.ToolTip = "Must be greater than or equal to zero.";
				FeeValidator1.Foreground = Brushes.Orange;
				Fee.BorderBrush = Brushes.Orange;

				ret = false;
			}
			if (MassType.Text == "All Souls" || MassType.Text == "Soul/s of")
			{
				if (string.IsNullOrWhiteSpace(SoulsOf.Text))
				{
					SoulsofValidator.Visibility = Visibility.Visible;
					SoulsofValidator.ToolTip = "This field is required.";
					SoulsofValidator.Foreground = Brushes.Red;
					SoulsOf.BorderBrush = Brushes.Red;

					ret = false;
				}
			}
			return ret;
		}
		private void CreateMassRecord(object sender, RoutedEventArgs e)
		{
			if (TabControl1.SelectedIndex == 0)
			{
				if (CheckInputs() == true) {
					dbman = new DBConnectionManager();
					pmsutil = new PMSUtil();
					//TODO
					try
					{
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
							"UPDATE appointments SET appointment_date = @appointment_date, appointment_time = @appointment_time, appointment_type = @appointment_type, requested_by = @requested_by, placed_by = @placed_by, remarks = @remarks, status = @status WHERE appointment_id = @aid;";
						cmd.Parameters.AddWithValue("@aid", _aid);
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
						if (stat_code > 0)
						{
							_caller.SyncEvent2();
							MsgSuccess();
							this.Close();
						}
						else
						{
							MsgFail();
						}
						string tmp = pmsutil.LogScheduling(_aid, "LOGC-02");
					}
					catch (MySqlException ex)
					{
						
					}
					this.Close();
				}
				else {

				}
			}
			else
			{
				if (CheckInputs() == true)
				{
					string selTime = THours.Text + ":" + TMinutes.Text + " " + TimeMode.Text;

					if (IsAvailable(DateTime.Parse(SelectedDate2.Text).ToString("yyyy-MM-dd"), DateTime.Parse(selTime).ToString("HH:mm:ss"), GetPriest(AssignedPriest.Text)) == false)
					{
						MsgNotAvailable(DateTime.Parse(SelectedDate2.Text).ToString("MMM dd, yyyy"), DateTime.Parse(selTime).ToString("HH:mm tt"), AssignedPriest.Text);
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
								"UPDATE appointments SET appointment_date = @appointment_date, appointment_time = @appointment_time, appointment_type = @appointment_type, requested_by = @requested_by, placed_by = @placed_by, remarks = @remarks, status = @status WHERE appointment_id = @aid;";
							cmd.Parameters.AddWithValue("@aid", _aid);
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
							if (stat_code > 0)
							{
								_caller.SyncEvent2();
								MsgSuccess();
								this.Close();
							}
							else
							{
								MsgFail();
							}
							string tmp = pmsutil.LogScheduling(_aid, "LOGC-02");
						}
						catch (MySqlException ex)
						{
							
						}
					}
				}
				else {

				}
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
		private async void MsgNotAvailable(string date, string time, string priest)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Not available!", priest + " is not available on the selected date " + date + " " + time +". Please change accordingly and try again.");
		}
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "The schedule has been placed successfully.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
		private void UpdateFee2() {
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
		private void UpdateFee(object sender, EventArgs e)
		{
			UpdateFee2();
		}

		private void EnableDisableSoul(object sender, EventArgs e)
		{
			SoulsOf.Text = "";
			if (MassType.Text == "Soul/s of" || MassType.Text == "All Souls")
			{
				LabelLabel.Content = "Soul/s of";
				SoulsOf.IsEnabled = true;

			} else if (MassType.Text == "Funeral Mass") {
				LabelLabel.Content = "Remarks";
				SoulsOf.IsEnabled = true;
				SoulsOf.AppendText("Name of Deceased: ");
				SoulsOf.AppendText(Environment.NewLine);
				SoulsOf.AppendText("Venue of Mass: ");
			}
			else
			{
				SoulsOf.IsEnabled = false;
			}
			FetchMassFee();
		}

		private void SyncAvailablePriest(object sender, EventArgs e)
		{
			FetchPriests();
		}
	}
}
