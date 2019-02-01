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
    /// Interaction logic for AddAccountWindow.xaml
    /// </summary>
    public partial class EditEventTypeWindow : ChildWindow
    {
		private MySqlConnection conn;
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

		private string _tid;

		private EventTypes _caller;

        public EditEventTypeWindow(EventTypes caller, string tid)
        {
			_caller = caller;
            InitializeComponent();
			_tid = tid;
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
						cmd.CommandText = "SELECT * FROM appointment_types WHERE type_id = @tid LIMIT 1;";
						cmd.Parameters.AddWithValue("@tid", tid);
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								EType.Text = db_reader.GetString("appointment_type");
								Status.SelectedIndex = db_reader.GetInt32("custom") - 1;
								Fee.Value = db_reader.GetDouble("fee");
								Active.SelectedIndex = db_reader.GetInt32("status") - 1;
							}
						}
					}
				}
			}
		}
		private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
		private bool CheckInputs()
		{
			var bc = new BrushConverter();

			ETypeValidator.Visibility = Visibility.Hidden;
			ETypeValidator.Foreground = Brushes.Transparent;
			EType.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			EType.BorderBrush = Brushes.Red;
			bool ret = true;

			if (string.IsNullOrWhiteSpace(EType.Text))
			{
				ETypeValidator.Visibility = Visibility.Visible;
				ETypeValidator.ToolTip = "This field is required.";
				ETypeValidator.Foreground = Brushes.Red;
				EType.BorderBrush = Brushes.Red;

				ret = false;
			}
			return ret;
		}
		private void EditEventTypeButton_Click(object sender, RoutedEventArgs e)
		{
			if (CheckInputs() == true) {
				dbman = new DBConnectionManager();
				pmsutil = new PMSUtil();
				using (conn = new MySqlConnection(dbman.GetConnStr()))
				{
					conn.Open();
					if (conn.State == ConnectionState.Open)
					{
						MySqlCommand cmd = conn.CreateCommand();
						cmd = conn.CreateCommand();
						cmd.CommandText =
						"UPDATE appointment_types SET appointment_type = @appointment_type, custom = @custom, fee = @fee, status = @status WHERE type_id = @tid";
						cmd.Prepare();
						cmd.Parameters.AddWithValue("@tid", _tid);
						cmd.Parameters.AddWithValue("@appointment_type", EType.Text);
						cmd.Parameters.AddWithValue("@custom", Status.SelectedIndex + 1);
						cmd.Parameters.AddWithValue("@fee", Fee.Value);
						cmd.Parameters.AddWithValue("@status", Active.SelectedIndex + 1);
						int stat_code = cmd.ExecuteNonQuery();
						conn.Close();
						if (stat_code > 0)
						{
							_caller.SyncEventTypes();
							pmsutil.LogAccount("Edited Event Type: " + EType.Text);
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
			else {

			}
		}
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "The event/service/appointment type has been updated successfully.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
	}
}
