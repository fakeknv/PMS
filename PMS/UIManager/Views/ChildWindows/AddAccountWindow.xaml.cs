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
    public partial class AddAccountWindow : ChildWindow
    {
		private MySqlConnection conn;
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

		private string priv;

        public AddAccountWindow()
        {
            InitializeComponent();
			AccountType.SelectionChanged += EnableCustom;

		}
		private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
		private bool CheckInputs()
		{
			var bc = new BrushConverter();

			AccountTypeValidator.Visibility = Visibility.Hidden;
			AccountTypeValidator.Foreground = Brushes.Transparent;
			AccountType.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			EmpNameValidator.Visibility = Visibility.Hidden;
			EmpNameValidator.Foreground = Brushes.Transparent;
			EmpName.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			UsernameValidator.Visibility = Visibility.Hidden;
			UsernameValidator.Foreground = Brushes.Transparent;
			Username.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			PasswordValidator.Visibility = Visibility.Hidden;
			PasswordValidator.Foreground = Brushes.Transparent;
			Password.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			bool ret = true;

			if (string.IsNullOrWhiteSpace(AccountType.Text))
			{
				AccountTypeValidator.Visibility = Visibility.Visible;
				AccountTypeValidator.ToolTip = "This field is requried.";
				AccountTypeValidator.Foreground = Brushes.Red;
				AccountType.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(EmpName.Text))
			{
				EmpNameValidator.Visibility = Visibility.Visible;
				EmpNameValidator.ToolTip = "This field is requried.";
				EmpNameValidator.Foreground = Brushes.Red;
				EmpName.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Username.Text))
			{
				UsernameValidator.Visibility = Visibility.Visible;
				UsernameValidator.ToolTip = "This field is requried.";
				UsernameValidator.Foreground = Brushes.Red;
				Username.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Password.Text))
			{
				PasswordValidator.Visibility = Visibility.Visible;
				PasswordValidator.ToolTip = "This field is requried.";
				PasswordValidator.Foreground = Brushes.Red;
				Password.BorderBrush = Brushes.Red;

				ret = false;
			}

			//FIELD LEVEL VALIDATION
			if (Password.Text.Length < 5)
			{
				PasswordValidator.Visibility = Visibility.Visible;
				PasswordValidator.ToolTip = "Password should be 5 characters long or more.";
				PasswordValidator.Foreground = Brushes.Red;
				Password.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (Username.Text.Length < 5)
			{
				UsernameValidator.Visibility = Visibility.Visible;
				UsernameValidator.ToolTip = "Username should be 5 characters long or more.";
				UsernameValidator.Foreground = Brushes.Red;
				Username.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (CheckDupli() == true)
			{
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
					cmd.CommandText = "SELECT COUNT(*) FROM accounts WHERE user_name = @username;";
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@username", Username.Text);
					using (MySqlDataReader db_reader = cmd.ExecuteReader())
					{
						while (db_reader.Read())
						{
							if (db_reader.GetInt32("COUNT(*)") > 0)
							{
								UsernameValidator.Visibility = Visibility.Visible;
								UsernameValidator.ToolTip = "Username already exists.";
								UsernameValidator.Foreground = Brushes.Red;
								Username.BorderBrush = Brushes.Red;
								return true;
							}
							else {
								return false;
							}
						}
					}
				}
			}
			return true;
		}
		private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
		{
			if (CheckInputs() == true) {
				if (AccountType.SelectedIndex == 3)
				{
					priv = "6";
					if (Priv1.IsChecked == true)
					{
						priv += "2";
					}
					if (Priv2.IsChecked == true)
					{
						priv += "3";
					}
					if (Priv3.IsChecked == true)
					{
						priv += "4";
					}
					dbman = new DBConnectionManager();
					pmsutil = new PMSUtil();
					using (conn = new MySqlConnection(dbman.GetConnStr()))
					{
						conn.Open();
						if (conn.State == ConnectionState.Open)
						{
							string uid = Application.Current.Resources["uid"].ToString();
							string[] dt = pmsutil.GetServerDateTime().Split(null);
							DateTime cDate = Convert.ToDateTime(dt[0]);
							DateTime cTime = DateTime.Parse(dt[1] + " " + dt[2]);
							string curDate = cDate.ToString("yyyy-MM-dd");
							string curTime = cTime.ToString("HH:mm:ss");

							string accID = pmsutil.GenAccountID();
							MySqlCommand cmd = conn.CreateCommand();
							cmd.CommandText =
							"INSERT INTO accounts(account_id, user_name, pass_key, account_type)" +
							"VALUES(@account_id, @user_name, @pass_key, @account_type)";
							cmd.Prepare();
							cmd.Parameters.AddWithValue("@account_id", accID);
							cmd.Parameters.AddWithValue("@user_name", Username.Text);
							cmd.Parameters.AddWithValue("@pass_key", SecurePasswordHasher.Hash(Password.Text));
							cmd.Parameters.AddWithValue("@account_type", Convert.ToInt32(priv));
							int stat_code = cmd.ExecuteNonQuery();
							conn.Close();

							conn.Open();
							cmd = conn.CreateCommand();
							cmd.CommandText =
							"INSERT INTO accounts_info(account_id, name, date_created, time_created, creator)" +
							"VALUES(@account_id, @empname, @date_created, @time_created, @creator)";
							cmd.Prepare();
							cmd.Parameters.AddWithValue("@account_id", accID);
							cmd.Parameters.AddWithValue("@emp_name", Username.Text);
							cmd.Parameters.AddWithValue("@date_created", curDate);
							cmd.Parameters.AddWithValue("@time_created", curTime);
							cmd.Parameters.AddWithValue("@creator", uid);
							stat_code = cmd.ExecuteNonQuery();
							conn.Close();
							if (stat_code > 0)
							{
								MsgSuccess();
								pmsutil.LogAccount("Created an account - Username: " + Username.Text + " Type: " + Convert.ToInt32(priv));
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
				else
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
							DateTime cDate = Convert.ToDateTime(dt[0]);
							DateTime cTime = DateTime.Parse(dt[1] + " " + dt[2]);
							string curDate = cDate.ToString("yyyy-MM-dd");
							string curTime = cTime.ToString("HH:mm:ss");

							string accID = pmsutil.GenAccountID();
							MySqlCommand cmd = conn.CreateCommand();
							cmd.CommandText =
							"INSERT INTO accounts(account_id, user_name, pass_key, account_type)" +
							"VALUES(@account_id, @user_name, @pass_key, @account_type)";
							cmd.Prepare();
							cmd.Parameters.AddWithValue("@account_id", accID);
							cmd.Parameters.AddWithValue("@user_name", Username.Text);
							cmd.Parameters.AddWithValue("@pass_key", SecurePasswordHasher.Hash(Password.Text));
							cmd.Parameters.AddWithValue("@account_type", Convert.ToInt32(AccountType.SelectedIndex + 1));
							int stat_code = cmd.ExecuteNonQuery();
							conn.Close();

							conn.Open();
							cmd = conn.CreateCommand();
							cmd.CommandText =
							"INSERT INTO accounts_info(account_id, name, date_created, time_created, creator)" +
							"VALUES(@account_id, @emp_name, @date_created, @time_created, @creator)";
							cmd.Prepare();
							cmd.Parameters.AddWithValue("@account_id", accID);
							cmd.Parameters.AddWithValue("@emp_name", Username.Text);
							cmd.Parameters.AddWithValue("@date_created", curDate);
							cmd.Parameters.AddWithValue("@time_created", curTime);
							cmd.Parameters.AddWithValue("@creator", uid);
							stat_code = cmd.ExecuteNonQuery();
							conn.Close();
							if (stat_code > 0)
							{
								MsgSuccess();
								pmsutil.LogAccount("Created an account - Username: " + Username.Text + " Type: " + Convert.ToInt32(priv));
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
			}
			else {

			}
		}
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "The account has been created successfully.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
		private void EnableCustom(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (AccountType.SelectedIndex == 3)
			{
				Priv1.IsEnabled = true;
				Priv2.IsEnabled = true;
				Priv3.IsEnabled = true;
			}
			else {
				Priv1.IsEnabled = false;
				Priv2.IsEnabled = false;
				Priv3.IsEnabled = false;
			}
		}
	}
}
