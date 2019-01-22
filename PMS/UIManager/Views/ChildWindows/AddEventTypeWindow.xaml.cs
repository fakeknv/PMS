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
    public partial class AddEventTypeWindow : ChildWindow
    {
		private MySqlConnection conn;
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

        public AddEventTypeWindow()
        {
            InitializeComponent();

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

			bool ret = true;

			if (string.IsNullOrWhiteSpace(EType.Text))
			{
				ETypeValidator.Visibility = Visibility.Visible;
				ETypeValidator.ToolTip = "Username cannot be empty!";
				ETypeValidator.Foreground = Brushes.Red;
				EType.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (CheckDupli() == true) {
				ETypeValidator.Visibility = Visibility.Visible;
				ETypeValidator.ToolTip = "Already exists!";
				ETypeValidator.Foreground = Brushes.Red;
				EType.BorderBrush = Brushes.Red;

				ret = false;
			}
			return ret;
		}
		internal bool CheckDupli()
		{
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(*) FROM appointment_types WHERE appointment_type = @atype";
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@atype", EType.Text);
					using (MySqlDataReader db_reader = cmd.ExecuteReader())
					{
						while (db_reader.Read())
						{
							if (db_reader.GetInt32("COUNT(*)") > 0) {
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		private void CreateEventTypeButton_Click(object sender, RoutedEventArgs e)
		{
			if (CheckInputs() == true)
			{
				dbman = new DBConnectionManager();
				pmsutil = new PMSUtil();
				using (conn = new MySqlConnection(dbman.GetConnStr()))
				{
					conn.Open();
					if (conn.State == ConnectionState.Open)
					{
						string eid = pmsutil.GenEventTypeID();
						MySqlCommand cmd = conn.CreateCommand();
						cmd.CommandText =
						"INSERT INTO appointment_types(type_id, appointment_type, custom, fee, status)" +
						"VALUES(@eid, @appointment_type, @custom, @fee, @status)";
						cmd.Prepare();
						cmd.Parameters.AddWithValue("@eid", eid);
						cmd.Parameters.AddWithValue("@appointment_type", EType.Text);
						cmd.Parameters.AddWithValue("@custom", Status.SelectedIndex + 1);
						cmd.Parameters.AddWithValue("@fee", Fee.Value);
						cmd.Parameters.AddWithValue("@status", Active.SelectedIndex + 1);
						int stat_code = cmd.ExecuteNonQuery();
						conn.Close();
						if (stat_code > 0)
						{
							pmsutil.LogAccount("Added event type - Type: " + EType.Text + " Fee: " + Fee.Value);
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
			await metroWindow.ShowMessageAsync("Success!", "The event/service/appointment type has been saved successfully.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
	}
}
