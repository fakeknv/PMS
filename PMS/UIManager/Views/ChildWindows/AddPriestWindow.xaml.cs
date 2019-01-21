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
    public partial class AddPriestWindow : ChildWindow
    {
		private MySqlConnection conn;
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

        public AddPriestWindow()
        {
            InitializeComponent();

		}
		private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
		private bool CheckInputs()
		{
			var bc = new BrushConverter();

			NameValidator.Visibility = Visibility.Hidden;
			NameValidator.Foreground = Brushes.Transparent;
			PriestName.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			bool ret = true;

			if (string.IsNullOrWhiteSpace(PriestName.Text))
			{
				NameValidator.Visibility = Visibility.Visible;
				NameValidator.ToolTip = "Username cannot be empty!";
				NameValidator.Foreground = Brushes.Red;
				PriestName.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (CheckDupli() == true)
			{
				NameValidator.Visibility = Visibility.Visible;
				NameValidator.ToolTip = "Username cannot be empty!";
				NameValidator.Foreground = Brushes.Red;
				PriestName.BorderBrush = Brushes.Red;

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
					cmd.CommandText = "SELECT COUNT(*) residing_priests WHERE priest_name = @pname";
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@pname", PriestName.Text);
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
		private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
		{
			if (CheckInputs() == true) {
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

						string pid = pmsutil.GenPriestID();
						MySqlCommand cmd = conn.CreateCommand();
						cmd.CommandText =
						"INSERT INTO residing_priests(priest_id, priest_name, priest_status, created_by, date_created, time_created)" +
						"VALUES(@priest_id, @priest_name, @priest_status, @created_by, @date_created, @time_created)";
						cmd.Prepare();
						cmd.Parameters.AddWithValue("@priest_id", pid);
						cmd.Parameters.AddWithValue("@priest_name", PriestName.Text);
						cmd.Parameters.AddWithValue("@priest_status", Status.Text);
						cmd.Parameters.AddWithValue("@created_by", uid);
						cmd.Parameters.AddWithValue("@date_created", curDate);
						cmd.Parameters.AddWithValue("@time_created", curTime);
						int stat_code = cmd.ExecuteNonQuery();
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
			else {

			}
		}
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "Priest info has been saved successfully.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
	}
}
