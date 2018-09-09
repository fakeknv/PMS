using System;
using System.Reflection;
using System.Windows;
using MahApps.Metro.Controls;
//MySQL
using System.Data;
using MySql.Data.MySqlClient;

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
        public void InitUIElements() {
            VersionName.Content = "Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginSpinner.Visibility = Visibility.Visible;
            try
            {
                string DBConnString = "Server=192.168.254.103;Database=PMS_db;Uid=PMS_app;Pwd=PMS2018!;SslMode=none";
                MySqlDataReader DBReader = null;

                MySqlConnection DBConn = new MySqlConnection(DBConnString);
                DBConn.Open();
                if (DBConn.State == ConnectionState.Open)
                {
                    string username = UsernameField.Text;
                    string password = PasswordField.Password;

                    string DBStm = "SELECT * FROM accounts WHERE user_name='" + username + "'";
                    MySqlCommand DBComm = new MySqlCommand(DBStm, DBConn);
                    DBReader = DBComm.ExecuteReader();
                    if (DBReader.HasRows)
                    {
                        while (DBReader.Read())
                        {
                            if (username == DBReader.GetString(1) && SecurePasswordHasher.Verify(password, DBReader.GetString(2)) == true)
                            {
                                LoginSpinner.Visibility = Visibility.Hidden;
                                Registrar registrar = new Registrar();
                                registrar.Show();
                                this.Close();
                            }
                            else
                            {
                                LoginSpinner.Visibility = Visibility.Hidden;
                                StatusLabel.Content = "Invalid credentials. Please try again.";
                            }
                        }
                    }
                    else
                    {
                        LoginSpinner.Visibility = Visibility.Hidden;
                        StatusLabel.Content = "Invalid credentials. Please try again.";
                    }
                    //StatusLabel.Content = "MySQL Connected!";
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
        }
    }
}
