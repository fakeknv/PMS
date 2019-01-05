using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using PMS.UIComponents;
using PMS.UIManager.Views.ChildWindows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PMS.UIManager.Views
{
	/// <summary>
	/// Interaction logic for DeceasedDirectory.xaml
	/// </summary>
	public partial class DeceasedDirectory : UserControl
	{
		private MySqlConnection conn;
		private DBConnectionManager dbman;

		private ObservableCollection<DirectoryPreviewItem> directories;
		private ObservableCollection<DirectoryPreviewItem> directories_final;

		private int items;

		public DeceasedDirectory()
		{
			InitializeComponent();
			
			items = Convert.ToInt32(ItemsPerPage.Text);
			SyncDirectory();
			ItemsPerPage.SelectionChanged += Update2;
			CurrentPage.ValueChanged += Update;
		}
		private void SyncDirectory() {
			directories = new ObservableCollection<DirectoryPreviewItem>();
			directories_final = new ObservableCollection<DirectoryPreviewItem>();

			ComboBoxItem ci = (ComboBoxItem)ItemsPerPage.SelectedItem;
			int itemsPerPage = Convert.ToInt32(ci.Content);
			int page = 1;
			int count = 0;

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
						cmd.CommandText = "SELECT * FROM records, burial_records WHERE records.record_id = burial_records.record_id;";
						MySqlDataReader db_reader = cmd.ExecuteReader();
						while (db_reader.Read())
						{

							directories.Add(new DirectoryPreviewItem()
							{
								RecordID = db_reader.GetString("record_id"),
								RName = db_reader.GetString("recordholder_fullname"),
								DateOfDeath = DateTime.Parse(db_reader.GetString("record_date")).ToString("MMM dd, yyyy"),
								DateOfBurial = DateTime.Parse(db_reader.GetString("burial_date")).ToString("MMM dd, yyyy"),
								PlaceOfIternment = db_reader.GetString("place_of_interment"),
								Page = page
							});
							count++;
							if (count == itemsPerPage)
							{
								page++;
								count = 0;
							}
						}
						foreach (var cur in directories)
						{
							if (cur.Page == CurrentPage.Value)
							{
								directories_final.Add(new DirectoryPreviewItem()
								{
									RecordID = cur.RecordID,
									RName = cur.RName,
									DateOfDeath = cur.DateOfDeath,
									DateOfBurial = cur.DateOfBurial,
									PlaceOfIternment = cur.PlaceOfIternment,
									Page = cur.Page
								});
							}
						}
						//close Connection
						conn2.Close();
						//AccountsItemContainer.Items.Clear();
						DirectoryItemContainer.Items.Refresh();
						DirectoryItemContainer.ItemsSource = directories_final;
						DirectoryItemContainer.Items.Refresh();
					}
				}
				else
				{

				}
				conn.Close();
			}
		}
		private void Update(object sender, RoutedPropertyChangedEventArgs<double?> e)
		{
			SyncDirectory();
		}
		private void Update2(object sender, SelectionChangedEventArgs e)
		{
			SyncDirectory();
		}
		private async void MsgNoItemSelected()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "There is no item selected. Please try again.");
		}
		private async void ViewInfoButton_Click(object sender, RoutedEventArgs e)
		{
			DirectoryPreviewItem dpi = (DirectoryPreviewItem)DirectoryItemContainer.SelectedItem;
			if (dpi == null)
			{
				MsgNoItemSelected();
			}
			else
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new ViewInfoWindow(dpi.RecordID), this.DirectoryMainGrid);
			}
		}
	}
}
