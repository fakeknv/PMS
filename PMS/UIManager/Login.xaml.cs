using System;
using System.Reflection;
using System.Windows;
using MahApps.Metro.Controls;
//MySQL
using System.Data;
using MySql.Data.MySqlClient;

using System.Diagnostics;
using System.Threading;

namespace PMS.UIManager
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class Login : MetroWindow
    {
		public string userID;

        public Login()
        {
            InitializeComponent();
            InitUIElements();
        }
        public void InitUIElements() {
            VersionName.Content = "Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

			LoginSpinner.Visibility = Visibility.Visible;
			try
            {

				//string DBConnString = "Server=192.168.254.103;Database=PMS_db;Uid=PMS_app;Pwd=PMS2018!;SslMode=none";
				string conn_str = "Server=localhost;Database=pms_db;Uid=pms;Pwd=pms2018!;SslMode=none";

                MySqlConnection conn = new MySqlConnection(conn_str);
				conn.Open();
                if (conn.State == ConnectionState.Open)
                {
					MySqlCommand cmd = conn.CreateCommand();
                    string username = UsernameField.Text;
                    string password = PasswordField.Password;

                    cmd.CommandText = "SELECT * FROM accounts WHERE user_name = @username";
					cmd.Parameters.AddWithValue("@username", username);
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (username == db_reader.GetString("user_name") && SecurePasswordHasher.Verify(password, db_reader.GetString("pass_key")) == true)
						{
							userID = db_reader.GetString("account_id");
							this.Close();
						}
						else
						{
							StatusLabel.Content = "Invalid credentials. Please try again.";
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
		}
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Process.GetCurrentProcess().Kill();
		}
	}
}
