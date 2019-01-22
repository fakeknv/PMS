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
    public partial class EditPriestWindow : ChildWindow
    {
		private MySqlConnection conn;
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

		private string pid;

        public EditPriestWindow(string p_id)
        {
			pid = p_id;
            InitializeComponent();

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
						cmd.CommandText = "SELECT * FROM residing_priests WHERE priest_id = @pid LIMIT 1;";
						cmd.Parameters.AddWithValue("@pid", pid);
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								PriestName.Text = db_reader.GetString("priest_name");
								if (db_reader.GetString("priest_status") == "Active")
								{
									Status.SelectedIndex = 0;
								}
								else
								{
									Status.SelectedIndex = 1;
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
		private bool CheckInputs()
		{
			bool ret = true;

			if (string.IsNullOrWhiteSpace(PriestName.Text))
			{
				PriestNameValidator.Visibility = Visibility.Visible;
				PriestNameValidator.ToolTip = "Username cannot be empty!";
				PriestNameValidator.Foreground = Brushes.Red;
				PriestName.BorderBrush = Brushes.Red;

				ret = false;
			}
			return ret;
		}
		private void EditPriestButton_Click(object sender, RoutedEventArgs e)
		{
			if (CheckInputs() == true) {
				dbman = new DBConnectionManager();
				pmsutil = new PMSUtil();
				using (conn = new MySqlConnection(dbman.GetConnStr()))
				{
					conn.Open();
					if (conn.State == ConnectionState.Open)
					{
						MySqlCommand cmd = conn.CreateCommand();
						cmd.CommandText =
						"UPDATE residing_priests SET priest_name = @priest_name, priest_status = @priest_status WHERE priest_id = @pid";
						cmd.Prepare();
						cmd.Parameters.AddWithValue("@pid", pid);
						cmd.Parameters.AddWithValue("@priest_name", PriestName.Text);
						cmd.Parameters.AddWithValue("@priest_status", Status.Text);
						int stat_code = cmd.ExecuteNonQuery();
						conn.Close();
						if (stat_code > 0)
						{
							pmsutil.LogAccount("Edited Priest: " + PriestName.Text);
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
