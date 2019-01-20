using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using PMS.UIComponents;
using PMS.UIManager.Views.ChildViews;
using PMS.UIManager.Views.ChildWindows;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PMS.UIManager.Views
{
	/// <summary>
	/// Interaction logic for Registers.xaml
	/// </summary>
	public partial class Archives : UserControl
	{
		//MYSQL
		private MySqlConnection conn;
		DBConnectionManager dbman;

		private ObservableCollection<RegisterItemArchive> registers;
		private ObservableCollection<RegisterItemArchive> registers_final;
		private PMSUtil pmsutil;

		private string archiveDrive = "init";

		private string path;

		public Archives()
		{
			InitializeComponent();
			SyncRegisters();

			ItemsPerPage.SelectionChanged += Update2;
			CurrentPage.ValueChanged += Update;

			path = @"\archive.db";

			Task task = new Task(() =>
			{
				while (true)
				{
					CheckArchiveDrive();
					Thread.Sleep(1000);
				}
			});
			task.Start();
		}
		private string CountEntries(int bookNum)
		{
			string ret = "0";
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM records WHERE book_number = @book_number;";
				cmd.Parameters.AddWithValue("@book_number", bookNum);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = db_reader.GetString("COUNT(*)");
				}
				//close Connection
				dbman.DBClose();
			}
			return ret;
		}
		internal int CheckIfArchived(int bookNum)
		{

			int returnVal = 0;

			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(book_number) FROM archives WHERE book_number = @book_number;";
				cmd.Parameters.AddWithValue("@book_number", bookNum);
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (db_reader.GetInt32("COUNT(book_number)") > 0)
					{
						//Archived
						returnVal = 1;
					}
					else
					{
						//Not Archived
						returnVal = 2;
					}
				}
				//close Connection
				dbman.DBClose();
			}
			return returnVal;
		}
		internal void CheckArchiveDrive()
		{
			DriveInfo[] allDrives = DriveInfo.GetDrives();
			pmsutil = new PMSUtil();
			if (pmsutil.CheckArchiveDrive(path) != "dc")
			{
				this.Dispatcher.Invoke(() =>
				{
					ArchiveStatus.Content = "Archive Status: Connected";
					archiveDrive = pmsutil.CheckArchiveDrive(path);
				});
			}
			else
			{
				this.Dispatcher.Invoke(() =>
				{
					ArchiveStatus.Content = "Archive Status: Disconnected";
					archiveDrive = "init";
				});
			}
			Thread.Sleep(5000); // 5sec
		}
		private string CheckFrequency(int bookNum)
		{
			string ret = "Never";
			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					//Yearly
					cmd.CommandText = "SELECT COUNT(*) FROM transactions, records WHERE transactions.target_id = records.record_id AND records.book_number = @book_number AND transactions.tran_date > @min AND transactions.tran_date < @max;";
					cmd.Parameters.AddWithValue("@book_number", bookNum);
					cmd.Parameters.AddWithValue("@min", DateTime.Now.ToString("yyyy-01-01"));
					cmd.Parameters.AddWithValue("@max", DateTime.Now.ToString("yyyy-12-31"));
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (db_reader.GetInt32("COUNT(*)") > 0)
						{
							ret = "Access Frequency: Low";
						}
					}
					conn.Close();

					conn.Open();
					cmd = conn.CreateCommand();
					//Monthly
					cmd.CommandText = "SELECT COUNT(*) FROM transactions, records WHERE transactions.target_id = records.record_id AND records.book_number = @book_number AND transactions.tran_date > @min AND transactions.tran_date < @max;";
					cmd.Parameters.AddWithValue("@book_number", bookNum);
					cmd.Parameters.AddWithValue("@min", DateTime.Now.ToString("yyyy-MM-01"));
					cmd.Parameters.AddWithValue("@max", DateTime.Now.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"));
					cmd.Prepare();
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (db_reader.GetInt32("COUNT(*)") > 0)
						{
							ret = "Moderate";
						}
					}
					conn.Close();

					conn.Open();
					cmd = conn.CreateCommand();
					//Weekly
					DayOfWeek day = DateTime.Now.DayOfWeek;
					int days = day - DayOfWeek.Monday;
					DateTime start = DateTime.Now.AddDays(-days);
					DateTime end = start.AddDays(6);

					cmd.CommandText = "SELECT COUNT(*) FROM transactions, records WHERE transactions.target_id = records.record_id AND records.book_number = @book_number AND transactions.tran_date > @min AND transactions.tran_date < @max;";
					cmd.Parameters.AddWithValue("@book_number", bookNum);
					cmd.Parameters.AddWithValue("@min", start.ToString("yyyy-MM-dd"));
					cmd.Parameters.AddWithValue("@max", end.ToString("yyyy-MM-dd"));
					cmd.Prepare();
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (db_reader.GetInt32("COUNT(*)") > 0)
						{
							ret = "High";
						}
					}
					//close Connection
					conn.Close();
				}
			}
			return ret;
		}
		/// <summary>
		/// Takes int as a parameter which is the target Register's Book Number. This function
		/// Counts the number of pages a register contains.
		/// </summary>
		private string CountPages(int bookNum)
		{
			string ret = "0";
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM records WHERE book_number = @book_number ORDER BY page_number DESC LIMIT 1;";
				cmd.Parameters.AddWithValue("@book_number", bookNum);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = db_reader.GetString("page_number");
				}
				//close Connection
				dbman.DBClose();
			}
			return ret;
		}
		/// <summary>
		/// Updates the Register list.
		/// </summary>
		internal void SyncRegisters()
		{
			registers = new ObservableCollection<RegisterItemArchive>();
			registers_final = new ObservableCollection<RegisterItemArchive>();

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
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM registers;";
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						int bookNum = Convert.ToInt32(db_reader.GetString("book_number"));
						RegisterItemArchive ri = new RegisterItemArchive();
						if (db_reader.GetString("status") == "Archived")
						{
							ri.RegIcon.Kind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.ArchiveSolid;
							ri.RegIcon.ToolTip = "This register is archived.";
						}
						ri.BookTypeHolder.Content = db_reader.GetString("book_type");
						ri.BookNoHolder.Content = "Book #" + db_reader.GetString("book_number");
						ri.BookContentStatHolder.Content = CountEntries(Convert.ToInt32(db_reader.GetString("book_number"))) + " Entries | " + CountPages(Convert.ToInt32(db_reader.GetString("book_number"))) + " Pages";
						ri.ArchiveRegisterButton.Click += (sender, e) => { ArchiveRegister_Click(sender, e, bookNum); };
						ri.RestoreRegisterButton.Click += (sender, e) => { RestoreRegister_Click(sender, e, bookNum); };
						ri.AccessFrequency.Content = "Access Frequency: " + CheckFrequency(bookNum);
						if (CheckFrequency(bookNum) == "Never")
						{
							ri.AccessFrequency.Foreground = Brushes.OrangeRed;
						}
						else if (CheckFrequency(bookNum) == "Low")
						{
							ri.AccessFrequency.Foreground = Brushes.Orange;
						}
						else if (CheckFrequency(bookNum) == "Moderate")
						{
							ri.AccessFrequency.Foreground = Brushes.ForestGreen;
						}
						else if (CheckFrequency(bookNum) == "High")
						{
							ri.AccessFrequency.Foreground = Brushes.DeepSkyBlue;
						}
						ri.Page.Content = page;
						registers.Add(ri);

						count++;
						if (count == itemsPerPage)
						{
							page++;
							count = 0;
						}
					}
					foreach (var cur in registers)
					{
						if (Convert.ToInt32(cur.Page.Content) == CurrentPage.Value)
						{
							registers_final.Add(cur);
						}
					}
					//close Connection
					conn.Close();

					RegistersItemContainer.Items.Refresh();
					RegistersItemContainer.ItemsSource = registers_final;
					RegistersItemContainer.Items.Refresh();
				}
				else
				{

				}
			}
		}
		/// <summary>
		/// Onclick event for the Baptismal Filter Button. Filters the Requests to show
		/// registers of type of Bapismal.
		/// </summary>
		private void ShowBaptismal_Click(object senderx, RoutedEventArgs ex)
		{
			registers = new ObservableCollection<RegisterItemArchive>();
			registers_final = new ObservableCollection<RegisterItemArchive>();

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
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM registers WHERE book_type = 'Baptismal';";
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						int bookNum = Convert.ToInt32(db_reader.GetString("book_number"));
						RegisterItemArchive ri = new RegisterItemArchive();
						if (db_reader.GetString("status") == "Archived")
						{
							ri.RegIcon.Kind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.ArchiveSolid;
							ri.RegIcon.ToolTip = "This register is archived.";
						}
						ri.BookTypeHolder.Content = db_reader.GetString("book_type");
						ri.BookNoHolder.Content = "Book #" + db_reader.GetString("book_number");
						ri.BookContentStatHolder.Content = CountEntries(Convert.ToInt32(db_reader.GetString("book_number"))) + " Entries | " + CountPages(Convert.ToInt32(db_reader.GetString("book_number"))) + " Pages";
						ri.ArchiveRegisterButton.Click += (sender, e) => { ArchiveRegister_Click(sender, e, bookNum); };
						ri.RestoreRegisterButton.Click += (sender, e) => { RestoreRegister_Click(sender, e, bookNum); };
						ri.AccessFrequency.Content = "Access Frequency: " + CheckFrequency(bookNum);
						if (CheckFrequency(bookNum) == "Never")
						{
							ri.AccessFrequency.Foreground = Brushes.OrangeRed;
						}
						else if (CheckFrequency(bookNum) == "Low")
						{
							ri.AccessFrequency.Foreground = Brushes.Orange;
						}
						else if (CheckFrequency(bookNum) == "Moderate")
						{
							ri.AccessFrequency.Foreground = Brushes.ForestGreen;
						}
						else if (CheckFrequency(bookNum) == "High")
						{
							ri.AccessFrequency.Foreground = Brushes.DeepSkyBlue;
						}
						ri.Page.Content = page;
						registers.Add(ri);

						count++;
						if (count == itemsPerPage)
						{
							page++;
							count = 0;
						}
					}
					foreach (var cur in registers)
					{
						if (Convert.ToInt32(cur.Page.Content) == CurrentPage.Value)
						{
							registers_final.Add(cur);
						}
					}
					//close Connection
					conn.Close();

					RegistersItemContainer.Items.Refresh();
					RegistersItemContainer.ItemsSource = registers_final;
					RegistersItemContainer.Items.Refresh();
				}
				else
				{

				}
			}
		}
		/// <summary>
		/// Onclick event for the Confirmation Filter Button. Filters the Requests to show
		/// registers of type of Confirmation.
		/// </summary>
		private void ShowConfirmation_Click(object senderx, RoutedEventArgs ex)
		{
			registers = new ObservableCollection<RegisterItemArchive>();
			registers_final = new ObservableCollection<RegisterItemArchive>();

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
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM registers WHERE book_type = 'Confirmation';";
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						int bookNum = Convert.ToInt32(db_reader.GetString("book_number"));
						RegisterItemArchive ri = new RegisterItemArchive();
						if (db_reader.GetString("status") == "Archived")
						{
							ri.RegIcon.Kind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.ArchiveSolid;
							ri.RegIcon.ToolTip = "This register is archived.";
						}
						ri.BookTypeHolder.Content = db_reader.GetString("book_type");
						ri.BookNoHolder.Content = "Book #" + db_reader.GetString("book_number");
						ri.BookContentStatHolder.Content = CountEntries(Convert.ToInt32(db_reader.GetString("book_number"))) + " Entries | " + CountPages(Convert.ToInt32(db_reader.GetString("book_number"))) + " Pages";
						ri.ArchiveRegisterButton.Click += (sender, e) => { ArchiveRegister_Click(sender, e, bookNum); };
						ri.RestoreRegisterButton.Click += (sender, e) => { RestoreRegister_Click(sender, e, bookNum); };
						ri.AccessFrequency.Content = "Access Frequency: " + CheckFrequency(bookNum);
						if (CheckFrequency(bookNum) == "Never")
						{
							ri.AccessFrequency.Foreground = Brushes.OrangeRed;
						}
						else if (CheckFrequency(bookNum) == "Low")
						{
							ri.AccessFrequency.Foreground = Brushes.Orange;
						}
						else if (CheckFrequency(bookNum) == "Moderate")
						{
							ri.AccessFrequency.Foreground = Brushes.ForestGreen;
						}
						else if (CheckFrequency(bookNum) == "High")
						{
							ri.AccessFrequency.Foreground = Brushes.DeepSkyBlue;
						}
						ri.Page.Content = page;
						registers.Add(ri);

						count++;
						if (count == itemsPerPage)
						{
							page++;
							count = 0;
						}
					}
					foreach (var cur in registers)
					{
						if (Convert.ToInt32(cur.Page.Content) == CurrentPage.Value)
						{
							registers_final.Add(cur);
						}
					}
					//close Connection
					conn.Close();

					RegistersItemContainer.Items.Refresh();
					RegistersItemContainer.ItemsSource = registers_final;
					RegistersItemContainer.Items.Refresh();
				}
				else
				{

				}
			}
		}
		/// <summary>
		/// Onclick event for the Matrimonial Filter Button. Filters the Requests to show
		/// registers of type of Matrimonial.
		/// </summary>
		private void ShowMatrimonial_Click(object senderx, RoutedEventArgs ex)
		{
			registers = new ObservableCollection<RegisterItemArchive>();
			registers_final = new ObservableCollection<RegisterItemArchive>();

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
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM registers WHERE book_type = 'Matrimonial';";
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						int bookNum = Convert.ToInt32(db_reader.GetString("book_number"));
						RegisterItemArchive ri = new RegisterItemArchive();
						if (db_reader.GetString("status") == "Archived")
						{
							ri.RegIcon.Kind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.ArchiveSolid;
							ri.RegIcon.ToolTip = "This register is archived.";
						}
						ri.BookTypeHolder.Content = db_reader.GetString("book_type");
						ri.BookNoHolder.Content = "Book #" + db_reader.GetString("book_number");
						ri.BookContentStatHolder.Content = CountEntries(Convert.ToInt32(db_reader.GetString("book_number"))) + " Entries | " + CountPages(Convert.ToInt32(db_reader.GetString("book_number"))) + " Pages";
						ri.ArchiveRegisterButton.Click += (sender, e) => { ArchiveRegister_Click(sender, e, bookNum); };
						ri.RestoreRegisterButton.Click += (sender, e) => { RestoreRegister_Click(sender, e, bookNum); };
						ri.AccessFrequency.Content = "Access Frequency: " + CheckFrequency(bookNum);
						if (CheckFrequency(bookNum) == "Never")
						{
							ri.AccessFrequency.Foreground = Brushes.OrangeRed;
						}
						else if (CheckFrequency(bookNum) == "Low")
						{
							ri.AccessFrequency.Foreground = Brushes.Orange;
						}
						else if (CheckFrequency(bookNum) == "Moderate")
						{
							ri.AccessFrequency.Foreground = Brushes.ForestGreen;
						}
						else if (CheckFrequency(bookNum) == "High")
						{
							ri.AccessFrequency.Foreground = Brushes.DeepSkyBlue;
						}
						ri.Page.Content = page;
						registers.Add(ri);

						count++;
						if (count == itemsPerPage)
						{
							page++;
							count = 0;
						}
					}
					foreach (var cur in registers)
					{
						if (Convert.ToInt32(cur.Page.Content) == CurrentPage.Value)
						{
							registers_final.Add(cur);
						}
					}
					//close Connection
					conn.Close();

					RegistersItemContainer.Items.Refresh();
					RegistersItemContainer.ItemsSource = registers_final;
					RegistersItemContainer.Items.Refresh();
				}
				else
				{

				}
			}
		}
		/// <summary>
		/// Onclick event for the Burial Filter Button. Filters the Requests to show
		/// registers of type of Burial.
		/// </summary>
		private void ShowBurial_Click(object senderx, RoutedEventArgs ex)
		{
			registers = new ObservableCollection<RegisterItemArchive>();
			registers_final = new ObservableCollection<RegisterItemArchive>();

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
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM registers WHERE book_type = 'Burial';";
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						int bookNum = Convert.ToInt32(db_reader.GetString("book_number"));
						RegisterItemArchive ri = new RegisterItemArchive();
						if (db_reader.GetString("status") == "Archived")
						{
							ri.RegIcon.Kind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.ArchiveSolid;
							ri.RegIcon.ToolTip = "This register is archived.";
						}
						ri.BookTypeHolder.Content = db_reader.GetString("book_type");
						ri.BookNoHolder.Content = "Book #" + db_reader.GetString("book_number");
						ri.BookContentStatHolder.Content = CountEntries(Convert.ToInt32(db_reader.GetString("book_number"))) + " Entries | " + CountPages(Convert.ToInt32(db_reader.GetString("book_number"))) + " Pages";
						ri.ArchiveRegisterButton.Click += (sender, e) => { ArchiveRegister_Click(sender, e, bookNum); };
						ri.RestoreRegisterButton.Click += (sender, e) => { RestoreRegister_Click(sender, e, bookNum); };
						ri.AccessFrequency.Content = "Access Frequency: " + CheckFrequency(bookNum);
						if (CheckFrequency(bookNum) == "Never")
						{
							ri.AccessFrequency.Foreground = Brushes.OrangeRed;
						}
						else if (CheckFrequency(bookNum) == "Low")
						{
							ri.AccessFrequency.Foreground = Brushes.Orange;
						}
						else if (CheckFrequency(bookNum) == "Moderate")
						{
							ri.AccessFrequency.Foreground = Brushes.ForestGreen;
						}
						else if (CheckFrequency(bookNum) == "High")
						{
							ri.AccessFrequency.Foreground = Brushes.DeepSkyBlue;
						}
						ri.Page.Content = page;
						registers.Add(ri);

						count++;
						if (count == itemsPerPage)
						{
							page++;
							count = 0;
						}
					}
					foreach (var cur in registers)
					{
						if (Convert.ToInt32(cur.Page.Content) == CurrentPage.Value)
						{
							registers_final.Add(cur);
						}
					}
					//close Connection
					conn.Close();

					RegistersItemContainer.Items.Refresh();
					RegistersItemContainer.ItemsSource = registers_final;
					RegistersItemContainer.Items.Refresh();
				}
				else
				{

				}
			}
		}
		/// <summary>
		/// Onclick event for the ManualSyncButton. Calls the SyncRegister function to manually
		/// sync registers.
		/// </summary>
		private void ManualSyncButton_Click(object sender, RoutedEventArgs e)
		{
			SyncRegisters();
		}
		/// <summary>
		/// Onclick event for the ViewRegisterButton. Shows the ViewRegisterWindow that allows 
		/// the user to add a register.
		/// </summary>
		private async void ArchiveRegister_Click(object sender, EventArgs e, int bookNum)
		{
			if (archiveDrive == "init")
			{
				MsgDriveNotFound();
			}
			else if (CheckIfArchived(bookNum) == 1)
			{
				MsgAlreadyArchived();
			}
			else
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new ConfirmArchivalWindow(this, bookNum, archiveDrive));
			}
		}
		private async void RestoreRegister_Click(object sender, EventArgs e, int bookNum)
		{
			if (archiveDrive == "init")
			{
				MsgDriveNotFound();
			}
			else if (CheckIfArchived(bookNum) == 1)
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new ConfirmRestoreWindow(this, bookNum, archiveDrive));
			}
			else
			{
				//MsgAlreadyArchived();
			}
		}
		private void UpdateContent(object senderx, TextChangedEventArgs ex)
		{
			registers = new ObservableCollection<RegisterItemArchive>();
			registers_final = new ObservableCollection<RegisterItemArchive>();

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
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM registers WHERE " +
					"book_number LIKE @query OR " +
					"book_type LIKE @query OR " +
					"creation_date LIKE @query OR " +
					"addition_date LIKE @query OR " +
					"status LIKE @query;";
					cmd.Parameters.AddWithValue("@query", "%" + SearchRegisterBox.Text + "%");
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						int bookNum = Convert.ToInt32(db_reader.GetString("book_number"));
						RegisterItemArchive ri = new RegisterItemArchive();
						if (db_reader.GetString("status") == "Archived")
						{
							ri.RegIcon.Kind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.ArchiveSolid;
							ri.RegIcon.ToolTip = "This register is archived.";
						}
						ri.BookTypeHolder.Content = db_reader.GetString("book_type");
						ri.BookNoHolder.Content = "Book #" + db_reader.GetString("book_number");
						ri.BookContentStatHolder.Content = CountEntries(Convert.ToInt32(db_reader.GetString("book_number"))) + " Entries | " + CountPages(Convert.ToInt32(db_reader.GetString("book_number"))) + " Pages";
						ri.ArchiveRegisterButton.Click += (sender, e) => { ArchiveRegister_Click(sender, e, bookNum); };
						ri.RestoreRegisterButton.Click += (sender, e) => { RestoreRegister_Click(sender, e, bookNum); };
						ri.AccessFrequency.Content = "Access Frequency: " + CheckFrequency(bookNum);
						if (CheckFrequency(bookNum) == "Never")
						{
							ri.AccessFrequency.Foreground = Brushes.OrangeRed;
						}
						else if (CheckFrequency(bookNum) == "Low")
						{
							ri.AccessFrequency.Foreground = Brushes.Orange;
						}
						else if (CheckFrequency(bookNum) == "Moderate")
						{
							ri.AccessFrequency.Foreground = Brushes.ForestGreen;
						}
						else if (CheckFrequency(bookNum) == "High")
						{
							ri.AccessFrequency.Foreground = Brushes.DeepSkyBlue;
						}
						ri.Page.Content = page;
						registers.Add(ri);

						count++;
						if (count == itemsPerPage)
						{
							page++;
							count = 0;
						}
					}
					foreach (var cur in registers)
					{
						if (Convert.ToInt32(cur.Page.Content) == CurrentPage.Value)
						{
							registers_final.Add(cur);
						}
					}
					//close Connection
					conn.Close();

					RegistersItemContainer.Items.Refresh();
					RegistersItemContainer.ItemsSource = registers_final;
					RegistersItemContainer.Items.Refresh();
				}
				else
				{

				}
			}
		}
		private async void MsgDriveNotFound()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "The Archive Drive is not detected. Please connect or reconnect the drive and try the action again.");
		}
		private async void MsgAlreadyArchived()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "The selected register is already archived.");
		}
		private void Update(object sender, RoutedPropertyChangedEventArgs<double?> e)
		{
			SyncRegisters();
		}
		private void Update2(object sender, SelectionChangedEventArgs e)
		{
			SyncRegisters();
		}
	}
}