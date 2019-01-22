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
using System.Data.SQLite;
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

namespace PMS.UIManager.Views.ChildViews
{
	/// <summary>
	/// Interaction logic for ViewRecordEntries.xaml
	/// </summary>
	public partial class ViewDirectoryEntriesFromSearch : UserControl
	{
		//MYSQL STUFF
		private MySqlConnection conn, conn2;
		private DBConnectionManager dbman;

		#pragma warning disable 0169
		private int booknum;
		#pragma warning restore 0169

		private ObservableCollection<DirEntry> items;
		private ObservableCollection<DirEntry> items_final;

		private string block;
		private PMSUtil pmsutil;

		private string _query;
		public ViewDirectoryEntriesFromSearch(string _block, string query)
		{
			_query = query;
			block = _block;
			InitializeComponent();

			Title.Content = "Search results for: " + query;
			ItemsPerPage.SelectionChanged += Update2;
			CurrentPage.ValueChanged += Update;

			SyncEntries();
		}
		internal bool IsArchived(int bookNum)
		{
			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM registers WHERE book_number = @bookNum;";
					cmd.Parameters.AddWithValue("@bookNum", bookNum);
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (db_reader.GetString("status") == "Archived") {
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
		private void SyncEntries()
		{
			items = new ObservableCollection<DirEntry>();
			items_final = new ObservableCollection<DirEntry>();

			ComboBoxItem ci = (ComboBoxItem)ItemsPerPage.SelectedItem;
			int itemsPerPage = Convert.ToInt32(ci.Content);
			int page = 1;
			int count = 0;

			string name = "";
			string bdate = "";
			string intermentPlace = "";

			string archiveDrive = "init";
			string path = @"\archive.db";

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM records, burial_records, burial_directory WHERE burial_records.record_id = records.record_id AND burial_directory.record_id = burial_records.record_id AND (records.recordholder_fullname LIKE @query OR records.parent1_fullname LIKE @query OR records.parent2_fullname LIKE @query OR burial_records.place_of_interment LIKE @query OR burial_records.cause_of_death);";
					cmd.Parameters.AddWithValue("@query", "%" + _query + "%");
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						Console.WriteLine("here");
						if (IsArchived(db_reader.GetInt32("book_number")) == true)
						{
							pmsutil = new PMSUtil();
							if (pmsutil.CheckArchiveDrive(path) != "dc")
							{
								archiveDrive = pmsutil.CheckArchiveDrive(path);
								SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
								{
									FailIfMissing = true,
									DataSource = archiveDrive
								};
								using (SQLiteConnection connection = new SQLiteConnection(connectionString.ToString()))
								{

									// open the connection:
									connection.Open();
									string stm = "SELECT * FROM burial_records WHERE record_id='" + db_reader.GetString("record_id") + "';";

									using (SQLiteCommand cmdx = new SQLiteCommand(stm, connection))
									{
										using (SQLiteDataReader rdr = cmdx.ExecuteReader())
										{
											while (rdr.Read())
											{
												DateTime dateOfBurial = Convert.ToDateTime(rdr["burial_date"].ToString());

												name = db_reader.GetString("recordholder_fullname");
												bdate = dateOfBurial.ToString("MMM dd, yyyy");
												intermentPlace = rdr["place_of_interment"].ToString();
											}
										}
									}

								}
							}
							else
							{
								name = db_reader.GetString("recordholder_fullname");
								bdate = "---";
								intermentPlace = "---";
							}
						}
						else
						{
							name = db_reader.GetString("recordholder_fullname");
							bdate = DateTime.Parse(db_reader.GetString("burial_date")).ToString("MMM dd, yyyy");
							intermentPlace = db_reader.GetString("place_of_interment");
						}
						items.Add(new DirEntry()
						{
							DirectoryID = db_reader.GetString("record_id"),
							RecordID = db_reader.GetString("record_id"),
							Lot = db_reader.GetString("lot"),
							Plot = db_reader.GetString("plot"),
							FName = name,
							PlaceOfInterment = intermentPlace,
							BurialDate = bdate,
							Page = page
						});
						count++;
						if (count == itemsPerPage)
						{
							page++;
							count = 0;
						}
					}
				}
			}
			foreach (var cur in items)
			{
				if (cur.Page == CurrentPage.Value)
				{
					items_final.Add(new DirEntry()
					{
						DirectoryID = cur.DirectoryID,
						RecordID = cur.RecordID,
						Lot = cur.Lot,
						Plot = cur.Plot,
						FName = cur.FName,
						PlaceOfInterment = cur.PlaceOfInterment,
						BurialDate = cur.BurialDate,
						Page = cur.Page
					});
				}
			}
			DirectoryItemsContainer.Items.Refresh();
			DirectoryItemsContainer.ItemsSource = items_final;
			DirectoryItemsContainer.Items.Refresh();
			CurrentPage.Maximum = page;
		}
		private void Update(object sender, RoutedPropertyChangedEventArgs<double?> e)
		{
			SyncEntries();
		}
		private void Update2(object sender, SelectionChangedEventArgs e)
		{
			SyncEntries();
		}
		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			this.Content = new DeceasedDirectory();
		}
		internal void UpdatePageContent1(object sender, RoutedPropertyChangedEventArgs<double?> e)
		{
			
		}
		private async void MsgNoItemSelected()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "There is no item selected. Please try again.");
		}
		private async void ViewEntry_Click(object sender, RoutedEventArgs e)
		{
			if (DirectoryItemsContainer.SelectedItem == null)
			{
				MsgNoItemSelected();
			}
			else {
				DirEntry dpi = (DirEntry)DirectoryItemsContainer.SelectedItem;
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
}
