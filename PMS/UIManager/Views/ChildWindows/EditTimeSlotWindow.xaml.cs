using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace PMS.UIManager.Views.ChildWindows
{
    /// <summary>
    /// Interaction logic for AddAccountWindow.xaml
    /// </summary>
    public partial class EditTimeSlotWindow : ChildWindow
    {
		private MySqlConnection conn;
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

		private string tid;

        public EditTimeSlotWindow(string t_id)
        {
			tid = t_id;
            InitializeComponent();
			SyncTimePicker();

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
						cmd.CommandText = "SELECT * FROM timeslots WHERE timeslot_id = @tid LIMIT 1;";
						cmd.Parameters.AddWithValue("@tid", tid);
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								if (Convert.ToInt32(DateTime.Parse(db_reader.GetString("timeslot")).ToString("HH")) == 0)
								{
									HourPicker.SelectedIndex = Convert.ToInt32(DateTime.Parse(db_reader.GetString("timeslot")).ToString("HH"));
								}
								else {
									HourPicker.SelectedIndex = Convert.ToInt32(DateTime.Parse(db_reader.GetString("timeslot")).ToString("HH")) - 1;
								}
								if (Convert.ToInt32(DateTime.Parse(db_reader.GetString("timeslot")).ToString("mm")) == 0)
								{
									MinutePicker.SelectedIndex = Convert.ToInt32(DateTime.Parse(db_reader.GetString("timeslot")).ToString("mm"));
								}
								else
								{
									MinutePicker.SelectedIndex = Convert.ToInt32(DateTime.Parse(db_reader.GetString("timeslot")).ToString("mm")) - 1;
								}
								if (DateTime.Parse(db_reader.GetString("timeslot")).ToString("tt") == "AM") {
									ModePicker.SelectedIndex = 0;
								}
								else {
									ModePicker.SelectedIndex = 1;
								}
								if (db_reader.GetString("status") == "Active")
								{
									Status.SelectedIndex = 0;
								}
								else
								{
									Status.SelectedIndex = 1;
								}
							}
						}
					}
				}
			}
		}
		private void SyncTimePicker()
		{
			for (int i = 1; i < 13; i++)
			{
				HourPicker.Items.Add(i.ToString("D2"));
			}
			for (int i = 0; i < 61; i++)
			{
				MinutePicker.Items.Add(i.ToString("D2"));
			}
		}
		private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}

		private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
		{
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{

					string selTime = HourPicker.Text + ":" + MinutePicker.Text + " " + ModePicker.Text;
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "UPDATE timeslots SET timeslot = @timeslot, status = @status WHERE timeslot_id = @tid;";
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@tid", tid);
					cmd.Parameters.AddWithValue("@timeslot", DateTime.Parse(selTime).ToString("HH:mm:ss"));
					cmd.Parameters.AddWithValue("@status", Status.Text);
					int stat_code = cmd.ExecuteNonQuery();
					conn.Close();
					if (stat_code > 0)
					{
						MsgSuccess();
						this.Close();
					}
					else
					{
						MsgFail();
					}
				}
				else
				{

				}
			}
		}
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "The timeslot has been updated successfully.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
	}
}
