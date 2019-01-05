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
		private void CreateEventTypeButton_Click(object sender, RoutedEventArgs e)
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
			await metroWindow.ShowMessageAsync("Success!", "The event/service/appointment type has been saved successfully.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
	}
}
