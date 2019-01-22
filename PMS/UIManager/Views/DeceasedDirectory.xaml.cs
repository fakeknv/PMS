using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using PMS.UIComponents;
using PMS.UIManager.Views.ChildViews;
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

		private ObservableCollection<DirectoryItem2> directories;
		private ObservableCollection<DirectoryItem2> directories_final;

		private int items;
		#pragma warning disable 0169
		private string block;
		#pragma warning restore 0169

		public DeceasedDirectory()
		{
			InitializeComponent();
			
			items = Convert.ToInt32(ItemsPerPage.Text);
			SyncDirectory();
			ItemsPerPage.SelectionChanged += Update2;
			CurrentPage.ValueChanged += Update;
		}
		private int CountEntries(string did) {
			int ret = 0;
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
						cmd.CommandText = "SELECT COUNT(*) FROM burial_directory WHERE directory_id = @did;";
						cmd.Parameters.AddWithValue("@did", did);
						MySqlDataReader db_reader = cmd.ExecuteReader();
						while (db_reader.Read())
						{
							ret = db_reader.GetInt32("COUNT(*)");
						}
					}
				}
			}
			return ret;
		}
		private void SyncDirectory() {
			directories = new ObservableCollection<DirectoryItem2>();
			directories_final = new ObservableCollection<DirectoryItem2>();

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
						cmd.CommandText = "SELECT * FROM burial_directory ORDER BY block ASC;";
						MySqlDataReader db_reader = cmd.ExecuteReader();
						while (db_reader.Read())
						{
							string block = db_reader.GetString("block");
							DirectoryItem2 di = new DirectoryItem2();
							di.BlockHolder.Text = "Block: " + db_reader.GetString("block");
							di.BlockContentStatHolder.Content = "Entries: " + CountEntries(db_reader.GetString("directory_id"));
							di.ViewDirectoryButton.Click += (sender, e) => { ViewDirectory_Click(sender, e, block); };
							di.Page.Content = page;
							directories.Add(di);

							count++;
							if (count == itemsPerPage)
							{
								page++;
								count = 0;
							}
						}
						foreach (var cur in directories)
						{
							if (Convert.ToInt32(cur.Page.Content) == CurrentPage.Value)
							{
								directories_final.Add(cur);
							}
						}
						//close Connection
						conn2.Close();
						//AccountsItemContainer.Items.Clear();
						DirectoryItemContainer.Items.Refresh();
						DirectoryItemContainer.ItemsSource = directories_final;
						DirectoryItemContainer.Items.Refresh();
						CurrentPage.Maximum = page;
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
		private async void MsgNoResult()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("No records found.", "The system cannot find any records for the provided search data. Please try again.");
		}
		private async void MsgNoItemSelected()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "There is no item selected. Please try again.");
		}
		private void ViewDirectory_Click(object sender, EventArgs e, string block)
		{
			// set the content
			this.Content = new ViewDirectoryEntries(block);
		}
		private void ManualSyncButton_Click(object sender, RoutedEventArgs e)
		{

		}
		internal bool CheckIfExist() {
			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(*) FROM records, burial_records, burial_directory WHERE burial_records.record_id = records.record_id AND burial_directory.record_id = burial_records.record_id AND (records.recordholder_fullname LIKE @query OR records.parent1_fullname LIKE @query OR records.parent2_fullname LIKE @query OR burial_records.place_of_interment LIKE @query OR burial_records.cause_of_death);";
					cmd.Parameters.AddWithValue("@query", "%"+ SearchIndexBox.Text + "%");
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (db_reader.GetInt32("COUNT(*)") > 0) {
							return true;
						}
						else {
							return false;
						}
					}
				}
			}
			return false;
		}
		private void SearchButton_Click(object sender, RoutedEventArgs e)
		{
			if (CheckIfExist() == false) {
				MsgNoResult();
			}
			else {
				// set the content
				this.Content = new ViewDirectoryEntriesFromSearch(block, SearchIndexBox.Text);
			}
		}
	}
}
