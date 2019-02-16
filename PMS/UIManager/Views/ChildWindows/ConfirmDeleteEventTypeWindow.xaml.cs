using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Data;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRequestWindow.xaml
	/// </summary>
	public partial class ConfirmDeleteEventTypeWindow : ChildWindow
	{
		//MYSQL Related Stuff
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private string eid;

		private EventTypes etype;
		private MySqlConnection conn;

		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public ConfirmDeleteEventTypeWindow(EventTypes sender,string etype_id)
		{
			eid = etype_id;
			etype = sender;
			pmsutil = new PMSUtil();
			InitializeComponent();
		}
		private void ConfirmPayment_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "DELETE FROM appointment_types WHERE appointment_id = @eid LIMIT 1;";
					cmd.Parameters.AddWithValue("@eid", eid);
					cmd.Prepare();
					int stat_code = cmd.ExecuteNonQuery();
					conn.Close();
					if (stat_code > 0)
					{
						etype.SyncEventTypes();
						MsgSuccess();
						this.Close();
					}
					else
					{
						InfoArea.Foreground = new SolidColorBrush(Colors.Red);
						InfoArea.Content = "Unable to Delete Selected Event Type!";
					}
				}
			}
		}
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "The selected event type has been successfully deleted.");
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
