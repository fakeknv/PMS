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
    public partial class EditAccountWindow : ChildWindow
    {
		private MySqlConnection conn;
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

		private string priv;
		private string aid;

        public EditAccountWindow(string a_id)
        {
			aid = a_id;
            InitializeComponent();
			AccountType.SelectionChanged += EnableCustom;

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
						cmd.CommandText = "SELECT * FROM accounts, accounts_info WHERE accounts.account_id = @aid AND accounts.account_id = accounts_info.account_id LIMIT 1;";
						cmd.Parameters.AddWithValue("@aid", aid);
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								EmpName.Text = db_reader.GetString("name");
								Username.Text = db_reader.GetString("user_name");
								//Password.Text = ;
								if (db_reader.GetInt32("account_type") == 1) {
									AccountType.SelectedIndex = 0;
								}
								else if (db_reader.GetInt32("account_type") == 2)
								{
									AccountType.SelectedIndex = 1;
								}
								else if (db_reader.GetInt32("account_type") == 3)
								{
									AccountType.SelectedIndex = 2;
								}
								else if (db_reader.GetInt32("account_type") == 4)
								{
									AccountType.SelectedIndex = 3;
								}
								else
								{
									AccountType.SelectedIndex = 4;
									foreach (char c in db_reader.GetInt32("account_type").ToString()) {
										if (c == '2') {
											Priv1.IsChecked = true;
										}
										if (c == '3')
										{
											Priv2.IsChecked = true;
										}
										if (c == '4')
										{
											Priv3.IsChecked = true;
										}
									}
								}
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
		private bool VerifyKey(string uid) {
			bool ret = false;
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
						cmd.CommandText = "SELECT * FROM accounts WHERE accounts.account_id = @aid LIMIT 1;";
						cmd.Parameters.AddWithValue("@aid", uid);
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								if (SecurePasswordHasher.Verify(VerificationPass.Password, db_reader.GetString("pass_key")) == true)
								{
									ret = true;
								}
								else {
									ret = false;
								}
							}
						}
					}
				}
			}
			return ret;
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
			//if (string.IsNullOrWhiteSpace(Password.Text))
			//{
			//	PasswordValidator.Visibility = Visibility.Visible;
			//	PasswordValidator.ToolTip = "This field is requried.";
			//	PasswordValidator.Foreground = Brushes.Red;
			//	Password.BorderBrush = Brushes.Red;

			//	ret = false;
			//}
			if (string.IsNullOrWhiteSpace(VerificationPass.Password))
			{
				YourPasswordValidator.Visibility = Visibility.Visible;
				YourPasswordValidator.ToolTip = "This field is requried.";
				YourPasswordValidator.Foreground = Brushes.Red;
				VerificationPass.BorderBrush = Brushes.Red;

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
			return ret;
		}
		private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
		{
			if (CheckInputs() == true) {
				string uid = Application.Current.Resources["uid"].ToString();
				if (VerifyKey(uid) == true)
				{
					if (AccountType.SelectedIndex == 4)
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

								MySqlCommand cmd = conn.CreateCommand();
								if (string.IsNullOrWhiteSpace(Password.Text))
								{
									cmd.CommandText =
									"UPDATE accounts SET user_name = @user_name, account_type = @account_type WHERE account_id = @aid";
									cmd.Prepare();
									cmd.Parameters.AddWithValue("@aid", aid);
									cmd.Parameters.AddWithValue("@user_name", Username.Text);
									cmd.Parameters.AddWithValue("@account_type", Convert.ToInt32(priv));
								}
								else
								{
									cmd.CommandText =
									"UPDATE accounts SET user_name = @user_name, pass_key = @pass_key, account_type = @account_type WHERE account_id = @aid";
									cmd.Prepare();
									cmd.Parameters.AddWithValue("@aid", aid);
									cmd.Parameters.AddWithValue("@user_name", Username.Text);
									cmd.Parameters.AddWithValue("@pass_key", SecurePasswordHasher.Hash(Password.Text));
									cmd.Parameters.AddWithValue("@account_type", Convert.ToInt32(priv));
								}

								int stat_code = cmd.ExecuteNonQuery();
								conn.Close();

								conn.Open();
								cmd = conn.CreateCommand();
								cmd.CommandText =
								"UPDATE accounts_info SET name = @emp_name WHERE account_id = @aid";
								cmd.Prepare();
								cmd.Parameters.AddWithValue("@aid", aid);
								cmd.Parameters.AddWithValue("@emp_name", Username.Text);
								stat_code = cmd.ExecuteNonQuery();
								conn.Close();
								if (stat_code > 0)
								{
									pmsutil.LogAccount("Edited account: " + Username.Text);
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
					else
					{
						dbman = new DBConnectionManager();
						pmsutil = new PMSUtil();
						using (conn = new MySqlConnection(dbman.GetConnStr()))
						{
							conn.Open();
							if (conn.State == ConnectionState.Open)
							{
								MySqlCommand cmd = conn.CreateCommand();
								if (string.IsNullOrWhiteSpace(Password.Text))
								{
									cmd.CommandText =
									"UPDATE accounts SET user_name = @user_name, account_type = @account_type WHERE account_id = @aid";
									cmd.Prepare();
									cmd.Parameters.AddWithValue("@aid", aid);
									cmd.Parameters.AddWithValue("@user_name", Username.Text);
									cmd.Parameters.AddWithValue("@account_type", Convert.ToInt32(AccountType.SelectedIndex + 1));
								}
								else
								{
									cmd.CommandText =
									"UPDATE accounts SET user_name = @user_name, pass_key = @pass_key, account_type = @account_type WHERE account_id = @aid";
									cmd.Prepare();
									cmd.Parameters.AddWithValue("@aid", aid);
									cmd.Parameters.AddWithValue("@user_name", Username.Text);
									cmd.Parameters.AddWithValue("@pass_key", SecurePasswordHasher.Hash(Password.Text));
									cmd.Parameters.AddWithValue("@account_type", Convert.ToInt32(AccountType.SelectedIndex + 1));
								}

								int stat_code = cmd.ExecuteNonQuery();
								conn.Close();

								conn.Open();
								cmd = conn.CreateCommand();
								cmd.CommandText =
								"UPDATE accounts_info SET name = @emp_name WHERE account_id = @aid";
								cmd.Prepare();
								cmd.Parameters.AddWithValue("@aid", aid);
								cmd.Parameters.AddWithValue("@emp_name", Username.Text);
								stat_code = cmd.ExecuteNonQuery();
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
				}
				else
				{
					MsgWrongKey();
				}
			}
			else {

			}
		}
		private async void MsgWrongKey()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Verification Failed!", "You have entered your password incorrectly. Please check your input and try again.");
		}
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "The account info has been updated successfully.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
		private void EnableCustom(object sender, SelectionChangedEventArgs e)
		{
			e.Handled = true;
			if (AccountType.SelectedIndex == 4)
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
