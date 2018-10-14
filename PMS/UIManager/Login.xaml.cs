using System;
using System.Reflection;
using System.Windows;
using MahApps.Metro.Controls;
//MySQL
using System.Data;
using MySql.Data.MySqlClient;

using System.Diagnostics;

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
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Process.GetCurrentProcess().Kill();
		}
	}
}
