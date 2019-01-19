using System;
using System.Reflection;
using System.Windows;
using MahApps.Metro.Controls;
//MySQL
using System.Data;
using MySql.Data.MySqlClient;

using System.Diagnostics;
using System.Windows.Media;

namespace PMS.UIManager
{
	/// <summary>
	/// Interaction logic for LoginScreen.xaml
	/// </summary>
	public partial class Login : MetroWindow
	{
		public Login()
		{
			InitializeComponent();
			InitUIElements();
		}
		private void InitUIElements() {
			VersionName.Content = "Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}
		private void LoginButton_Click(object sender, RoutedEventArgs e)
		{

			if (CheckInputs() == true)
			{
				DBConnectionManager dbman = new DBConnectionManager();

				LoginSpinner.Visibility = Visibility.Visible;
				try
				{
					if (dbman.DBConnect().State == ConnectionState.Open)
					{
						StatusLabel.Content = "Invalid credentials. Please try again.";

						MySqlCommand cmd = dbman.DBConnect().CreateCommand();
						string username = UsernameField.Text;
						string password = PasswordField.Password;

						cmd.CommandText = "SELECT * FROM accounts WHERE user_name = @username";
						cmd.Parameters.AddWithValue("@username", username);
						MySqlDataReader db_reader = cmd.ExecuteReader();
						while (db_reader.Read())
						{
							if (username == db_reader.GetString("user_name") && SecurePasswordHasher.Verify(password, db_reader.GetString("pass_key")) == true)
							{
								Application.Current.Resources["uid"] = db_reader.GetString("account_id");
								dbman.DBClose();
								this.Close();
							}
						}
					}
					else
					{
						StatusLabel.Content = "DB Connection Failed!";
					}
				}
				catch (MySqlException ex)
				{
					StatusLabel.Content = "Error " + ex.Message;
				}
				LoginSpinner.Visibility = Visibility.Hidden;

				dbman.DBClose();
			}
			else {

			}
		}
		private bool CheckInputs() {
			UsernameValidator.Visibility = Visibility.Hidden;
			UsernameValidator.Foreground = Brushes.Transparent;
			UsernameField.BorderBrush = Brushes.Transparent;

			PasswordValidator.Visibility = Visibility.Hidden;
			PasswordValidator.Foreground = Brushes.Transparent;
			PasswordField.BorderBrush = Brushes.Transparent;

			bool ret = true;

			if (string.IsNullOrWhiteSpace(UsernameField.Text))
			{
				UsernameValidator.Visibility = Visibility.Visible;
				UsernameValidator.ToolTip = "Username cannot be empty!";
				UsernameValidator.Foreground = Brushes.Red;
				UsernameField.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(PasswordField.Password))
			{
				PasswordValidator.Visibility = Visibility.Visible;
				PasswordValidator.ToolTip = "Password cannot be empty!";
				PasswordValidator.Foreground = Brushes.Red;
				PasswordField.BorderBrush = Brushes.Red;

				ret = false;
			}
			return ret;
		}
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Process.GetCurrentProcess().Kill();
		}
	}
}
