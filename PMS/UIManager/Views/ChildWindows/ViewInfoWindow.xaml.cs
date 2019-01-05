using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PMS.UIManager.Views.ChildWindows
{
    /// <summary>
    /// Interaction logic for AddAccountWindow.xaml
    /// </summary>
    public partial class ViewInfoWindow : ChildWindow
    {
		private MySqlConnection conn;
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

		private string _rid;

        public ViewInfoWindow(string rid)
        {
			_rid = rid;
            InitializeComponent();
			pmsutil = new PMSUtil();

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM records, burial_records, burial_directory WHERE records.record_id = burial_records.record_id AND burial_records.record_id = burial_directory.record_id AND burial_directory.record_id = @rid LIMIT 1;";
					cmd.Parameters.AddWithValue("@rid", rid);
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						NameHolder.Content = "Name: " + db_reader.GetString("recordholder_fullname");
						AgeHolder.Content = "Age: " + db_reader.GetInt32("age");
						ResidenceHolder.Content = db_reader.GetString("residence");
						Parent1Holder.Content = db_reader.GetString("parent1_fullname");
						Parent2Holder.Content = db_reader.GetString("parent2_fullname");
						DateOfDeathHolder.Content = DateTime.Parse(db_reader.GetString("record_date")).ToString("MMMM dd, yyyy");
						BurialDateHolder.Content = DateTime.Parse(db_reader.GetString("burial_date")).ToString("MMMM dd, yyyy");
						CauseOfDeathHolder.Content = "Cause of Death: " + db_reader.GetString("cause_of_death");
						SacramentHolder.Content = "Sacrament: " + db_reader.GetString("sacrament");
						PlaceOfIntermentHolder.Text = db_reader.GetString("place_of_interment");

					}
					conn.Close();

					conn.Open();
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT gravestone FROM burial_directory WHERE burial_directory.record_id = @rid LIMIT 1;";
					cmd.Parameters.AddWithValue("@rid", rid);
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						var tmpy = db_reader.GetOrdinal("gravestone");
						if (db_reader.IsDBNull(tmpy))
						{
							//Load default img
						}
						else {
							byte[] data = (byte[])db_reader[0];
							using (System.IO.MemoryStream ms = new System.IO.MemoryStream(data))
							{
								var imageSource = new BitmapImage();
								imageSource.BeginInit();
								imageSource.StreamSource = ms;
								imageSource.CacheOption = BitmapCacheOption.OnLoad;
								imageSource.EndInit();

								// Assign the Source property of your image
								GravePicture.Source = imageSource;
							}
						}
					}
					conn.Close();

					conn.Open();
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT block, lot, plot FROM burial_directory WHERE burial_directory.record_id = @rid LIMIT 1;";
					cmd.Parameters.AddWithValue("@rid", rid);
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						BlockHolder.Content = db_reader.GetString("block");
						LotHolder.Content = db_reader.GetString("lot");
						PlotHolder.Content = db_reader.GetString("plot");
					}
					conn.Close();
				}
			}
		}
		private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
		private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
		{
			
		}
		private async void UpdateInfo_Click(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new EditDirectoryWindow(_rid));
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
	}
}
