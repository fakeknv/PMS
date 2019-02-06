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
	public partial class ConfirmPaymentWindow : ChildWindow
	{
		//MYSQL Related Stuff
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private string tid;
		private string ornum;

		private DateTime cDate;
		private DateTime cTime;
		private string curDate;
		private string curTime;

		private Transactions trans1;
		private MySqlConnection conn;
		private MySqlConnection conn2;

		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public ConfirmPaymentWindow(Transactions trans,string transaction_id)
		{
			tid = transaction_id;
			trans1 = trans;
			pmsutil = new PMSUtil();
			InitializeComponent();
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
					cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE or_number = @or";
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@atype", OR.Text);
					using (MySqlDataReader db_reader = cmd.ExecuteReader())
					{
						while (db_reader.Read())
						{
							if (db_reader.GetInt32("COUNT(*)") > 0)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		/// <summary>
		/// Inserts the request to the database.
		/// </summary>
		private int UpdateTransaction()
		{
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					string uid = Application.Current.Resources["uid"].ToString();
					string[] dt = pmsutil.GetServerDateTime().Split(null);
					cDate = Convert.ToDateTime(dt[0]);
					cTime = DateTime.Parse(dt[1] + " " + dt[2]);
					curDate = cDate.ToString("yyyy-MM-dd");
					curTime = cTime.ToString("HH:mm:ss");

					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText =
						"UPDATE transactions SET or_number = @or_number, status = @status, completion_date = @completion_date, completion_time = @completion_time, completed_by = @completed_by WHERE transaction_id = @transaction_id;";
					cmd.Parameters.AddWithValue("@transaction_id", tid);
					cmd.Parameters.AddWithValue("@status", "Paid");
					cmd.Parameters.AddWithValue("@completion_date", cDate);
					cmd.Parameters.AddWithValue("@completion_time", cTime);
					cmd.Parameters.AddWithValue("@completed_by", uid);
					cmd.Parameters.AddWithValue("@or_number", ornum);
					cmd.Prepare();
					int stat_code = cmd.ExecuteNonQuery();
					conn.Close();
					return stat_code;
				}
			}
			return 0;
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
					cmd.CommandText = "SELECT * FROM transactions WHERE transaction_id = @transaction_id LIMIT 1;";
					cmd.Parameters.AddWithValue("@transaction_id", tid);
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (db_reader.GetString("status") == "Paying")
						{
							ornum = OR.Text;

							using (conn2 = new MySqlConnection(dbman.GetConnStr()))
							{
								conn2.Open();
								MySqlCommand cmd2 = conn2.CreateCommand();
								cmd2.CommandText = "SELECT COUNT(*) FROM transactions WHERE or_number = @or_number;";
								cmd2.Parameters.AddWithValue("@or_number", ornum);
								cmd2.Prepare();
								int counter = int.Parse(cmd2.ExecuteScalar().ToString());
								if (counter > 0)
								{
									InfoArea.Foreground = new SolidColorBrush(Colors.Red);
									InfoArea.Content = "Duplicate Receipt Number Detected!";
								}
								else
								{
									UpdateTransaction();
									trans1.SyncTransactions();
									MsgSuccess();
									this.Close();
								}
							}
						}
					}
				}
			}
		}
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "The selected transaction has been successfully confirmed.");
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
