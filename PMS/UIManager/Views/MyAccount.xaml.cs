using MySql.Data.MySqlClient;
using PMS.UIComponents;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PMS.UIManager.Views
{
	/// <summary>
	/// Interaction logic for Account.xaml
	/// </summary>
	public partial class MyAccount : UserControl
	{
		private MySqlConnection conn;

		private PMSUtil pmsutil;
		private DBConnectionManager dbman;

		private ObservableCollection<MyLogsEntry> entries;

		public MyAccount()
		{
			pmsutil = new PMSUtil();
			InitializeComponent();
			NameTextbox.Text = pmsutil.GetFullName(Application.Current.Resources["uid"].ToString());
			SyncMyLogs();
			AccountNameHolder.Content = pmsutil.GetFullName(Application.Current.Resources["uid"].ToString());
			AccountRoleHolder.Content = pmsutil.GetAccountType(Application.Current.Resources["uid"].ToString());
		}
		internal void SyncMyLogs() {
			string uid = Application.Current.Resources["uid"].ToString();

			entries = new ObservableCollection<MyLogsEntry>();

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
						cmd.CommandText = "SELECT * FROM account_logs WHERE account_id = @aid;";
						cmd.Parameters.AddWithValue("@aid", uid);
						MySqlDataReader db_reader = cmd.ExecuteReader();
						while (db_reader.Read())
						{
							entries.Add(new MyLogsEntry()
							{
								Details = db_reader.GetString("log_details"),
								Date = DateTime.Parse(db_reader.GetString("log_date")).ToString("MMM dd, yyyy"),
								Time = DateTime.Parse(db_reader.GetString("log_time")).ToString("hh:mm tt")
							});
						}
					}
				}
			}
			LogHolder.Items.Refresh();
			LogHolder.ItemsSource = entries;
			LogHolder.Items.Refresh();
		}
		private void SaveButton_Click1(object sender, RoutedEventArgs e)
		{
			dbman = new DBConnectionManager();

			string uid = Application.Current.Resources["uid"].ToString();
			string old_pass_key = CurrentPassword.Password;
			string pass_key = SecurePasswordHasher.Hash(NewPassword1.Password);

			if (NewPassword1.Password == NewPassword2.Password)
			{
				if (dbman.DBConnect().State == ConnectionState.Open)
				{
					MySqlCommand cmd = dbman.DBConnect().CreateCommand();
					cmd.CommandText = "SELECT * FROM accounts WHERE account_id = @uid LIMIT 1;";
					cmd.Parameters.AddWithValue("@uid", uid);
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (SecurePasswordHasher.Verify(old_pass_key, db_reader.GetString("pass_key")) == true)
						{
							//TODO
							try
							{
								MySqlCommand cmd2 = dbman.DBConnect().CreateCommand();
								cmd2.CommandText =
									"UPDATE accounts SET pass_key = @pass_key WHERE account_id = @account_id;";
								cmd2.Prepare();
								cmd2.Parameters.AddWithValue("@account_id", uid);
								cmd2.Parameters.AddWithValue("@pass_key", pass_key);
								int stat_code = cmd2.ExecuteNonQuery();
								//string tmp = pmsutil.LogRecord(recordID, "LOGC-02");
								InfoArea1.Foreground = new SolidColorBrush(Colors.Green);
								InfoArea1.Content = "Password successfully changed!";
							}
							catch (MySqlException ex)
							{
								Console.WriteLine("Error: {0}", ex.ToString());
							}
						}
						else
						{
							InfoArea1.Foreground = new SolidColorBrush(Colors.Red);
							InfoArea1.Content = "Password does not match! Please check your inputs and try again.";
						}
					}
					//close Connection
					dbman.DBClose();
				}
				else
				{

				}
			}
			else
			{
				InfoArea1.Foreground = new SolidColorBrush(Colors.Red);
				InfoArea1.Content = "Password does not match! Please check your inputs and try again.";
			}
		}
		private void SaveButton_Click2(object sender, RoutedEventArgs e)
		{
			string uid = Application.Current.Resources["uid"].ToString();
			string fname = NameTextbox.Text;
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				//TODO
				try
				{
					MySqlCommand cmd = dbman.DBConnect().CreateCommand();
					cmd.CommandText =
						"UPDATE accounts_info SET name = @fname WHERE account_id = @account_id;";
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@account_id", uid);
					cmd.Parameters.AddWithValue("@fname", fname);
					int stat_code = cmd.ExecuteNonQuery();
					//string tmp = pmsutil.LogRecord(recordID, "LOGC-02");
					InfoArea2.Foreground = new SolidColorBrush(Colors.Green);
					InfoArea2.Content = "Name successfully changed!";

					//close Connection
					dbman.DBClose();
				}
				catch (MySqlException ex)
				{
					Console.WriteLine("Error: {0}", ex.ToString());
				}
			}
			else
			{

			}
		}
		private void ResetButton_Click1(object sender, RoutedEventArgs e)
		{
			CurrentPassword.Clear();
			NewPassword1.Clear();
			NewPassword2.Clear();
		}
		private void ResetButton_Click2(object sender, RoutedEventArgs e)
		{
			NameTextbox.Text = pmsutil.GetFullName(Application.Current.Resources["uid"].ToString());
		}
	}
}