using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PMS.UIManager.Views.ChildWindows
{
    /// <summary>
    /// Interaction logic for AddAccountWindow.xaml
    /// </summary>
    public partial class EditDirectoryWindow : ChildWindow
    {
		private MySqlConnection conn;
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

		private string imageURI;
		private string _rid;

        public EditDirectoryWindow(string rid)
        {
			_rid = rid;
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
						cmd.CommandText = "SELECT * FROM burial_directory WHERE record_id = @rid LIMIT 1;";
						cmd.Parameters.AddWithValue("@rid", rid);
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								Block.Text = db_reader.GetString("block");
								Lot.Text = db_reader.GetString("lot");
								Plot.Text = db_reader.GetString("plot");
								RContactNo.Text = db_reader.GetString("relative_contact_number");
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
		private void UpdateDirectoryButton_Click(object sender, RoutedEventArgs e)
		{
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					byte[] ImageData;

					if (!string.IsNullOrWhiteSpace(imageURI))
					{
						FileStream fs = new FileStream(imageURI, FileMode.Open, FileAccess.Read);
						BinaryReader br = new BinaryReader(fs);
						ImageData = br.ReadBytes((int)fs.Length);
						br.Close();
						fs.Close();
					}
					else
					{
						ImageData = null;
					}
					MySqlCommand cmd = conn.CreateCommand();
					cmd = conn.CreateCommand();
					cmd.CommandText =
					"UPDATE burial_directory SET block = @block, lot = @lot, plot = @plot, gravestone = @gravestone, relative_contact_number = @rcon WHERE record_id = @rid";
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@rid", _rid);
					cmd.Parameters.AddWithValue("@block", Block.Text);
					cmd.Parameters.AddWithValue("@lot", Lot.Text);
					cmd.Parameters.AddWithValue("@plot", Plot.Text);
					cmd.Parameters.AddWithValue("@gravestone", ImageData);
					cmd.Parameters.AddWithValue("@rcon", RContactNo.Text);
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
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "The directory info has been updated successfully.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
		private void ImagePicker_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog op = new OpenFileDialog
			{
				Title = "Select a picture",
				Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
			  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
			  "Portable Network Graphic (*.png)|*.png"
			};
			if (op.ShowDialog() == true)
			{
				ImagePreview.Source = new BitmapImage(new Uri(op.FileName));
				imageURI = op.FileName;
			}
		}
	}
}
