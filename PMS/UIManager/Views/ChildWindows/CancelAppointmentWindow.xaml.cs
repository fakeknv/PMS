using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Data;
using System.Windows.Media;
using System.Timers;
using System.Windows.Threading;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRequestWindow.xaml
	/// </summary>
	public partial class CancelAppointmentWindow : ChildWindow
	{
		//MYSQL Related Stuff
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private string aid;

		private DateTime cDate;
		private DateTime cTime;
		private string curDate;
		private string curTime;

		private ManageEventsWindow manWin;
		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public CancelAppointmentWindow(ManageEventsWindow sender, string appointment_id)
		{
			aid = appointment_id;
			manWin = sender;
			pmsutil = new PMSUtil();
			InitializeComponent();
		}
		/// <summary>
		/// Inserts the request to the database.
		/// </summary>
		private int UpdateTransaction()
		{
			string uid = Application.Current.Resources["uid"].ToString();
			string[] dt = pmsutil.GetServerDateTime().Split(null);
			cDate = Convert.ToDateTime(dt[0]);
			cTime = DateTime.Parse(dt[1] + " " + dt[2]);
			curDate = cDate.ToString("yyyy-MM-dd");
			curTime = cTime.ToString("HH:mm:ss");

			dbman = new DBConnectionManager();
			try
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText =
					"UPDATE transactions SET status = @status, completion_date = @completion_date, completion_time = @completion_time, completed_by = @completed_by WHERE target_id = @aid;";
				cmd.Parameters.AddWithValue("@aid", aid);
				cmd.Parameters.AddWithValue("@status", "Cancelled");
				cmd.Parameters.AddWithValue("@completion_date", cDate);
				cmd.Parameters.AddWithValue("@completion_time", cTime);
				cmd.Parameters.AddWithValue("@completed_by", uid);
				cmd.Prepare();
				int stat_code = cmd.ExecuteNonQuery();
				dbman.DBClose();
				return stat_code;
			}
			catch (MySqlException ex)
			{
				Console.WriteLine("Error: {0}", ex.ToString());
				return 0;
			}
		}
		/// <summary>
		/// Interaction logic for the AddRegConfirm button. Gathers and prepares the data
		/// for database insertion.
		/// </summary>
		private void CancelTransaction_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			UpdateTransaction();
			InfoArea.Foreground = new SolidColorBrush(Colors.Green);
			InfoArea.Content = "The selected schedule has been cancelled.";
			manWin.SyncEvent2();
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
