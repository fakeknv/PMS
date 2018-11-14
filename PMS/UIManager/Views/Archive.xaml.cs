using PMS.UIComponents;
using System.Windows.Controls;
using MahApps.Metro.SimpleChildWindow;
using System.Windows;
using MahApps.Metro.Controls;
using PMS.UIManager.Views.ChildWindows;
using System.IO;
using System.Linq;
using Dolinay;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using MahApps.Metro.Controls.Dialogs;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.UIManager.Views
{
    /// <summary>
    /// Interaction logic for Appointments.xaml
    /// </summary>
    public partial class Archive : UserControl
    {
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

		private string archiveDrive = "init";

		private string path;

		public Archive()
        {
            InitializeComponent();
			path = @"\archive.db";
			SyncRegisters();
			SyncArchives();

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
		internal void CheckArchiveDrive() {
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
			else {
				this.Dispatcher.Invoke(() =>
				{
					ArchiveStatus.Content = "Archive Status: Disconnected";
					archiveDrive = "init";
				});
			}
			Thread.Sleep(5000); // 5sec
		}
		internal void SyncRegisters()
		{
			dbman = new DBConnectionManager();

			RegistersItemContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM registers;";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					int bookNum = Convert.ToInt32(db_reader.GetString("book_number"));
					RegisterItemArchives ri = new RegisterItemArchives();
					ri.BookTypeHolder.Content = db_reader.GetString("book_type");
					ri.BookNoHolder.Content = "Book #" + db_reader.GetString("book_number");
					ri.BookContentStatHolder.Content = CountEntries(Convert.ToInt32(db_reader.GetString("book_number"))) + " Entries | " + CountPages(Convert.ToInt32(db_reader.GetString("book_number"))) + " Pages";
					ri.ArchiveRegisterButton.Click += (sender, e) => { ArchiveRegister_Click(sender, e, bookNum); };
					RegistersItemContainer.Items.Add(ri);
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}

		private async void ArchiveRegister_Click(object sender, RoutedEventArgs e, int bookNum)
		{
			if (archiveDrive == "init")
			{
				MsgDriveNotFound();
			}
			else if (CheckIfArchived(bookNum) == 1)
			{
				MsgAlreadyArchived();
			}
			else {
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new ConfirmArchivalWindow(this, bookNum, archiveDrive));
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
		internal int CheckIfArchived(int bookNum) {

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
					if (db_reader.GetInt32("COUNT(book_number)") > 0) {
						//Archived
						returnVal = 1;
					}
					else {
						//Not Archived
						returnVal = 2;
					}
				}
				//close Connection
				dbman.DBClose();
			}
			return returnVal;
		}
		internal void SyncArchives()
		{
			dbman = new DBConnectionManager();

			ArchivesItemContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM archives, registers WHERE archives.book_number = registers.book_number;";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					int bookNum = Convert.ToInt32(db_reader.GetString("book_number"));
					RegisterItemArchivesRestore ri = new RegisterItemArchivesRestore();
					ri.BookTypeHolder.Content = db_reader.GetString("book_type");
					ri.BookNoHolder.Content = "Book #" + db_reader.GetString("book_number");
					ri.BookContentStatHolder.Content = CountEntries(Convert.ToInt32(db_reader.GetString("book_number"))) + " Entries | " + CountPages(Convert.ToInt32(db_reader.GetString("book_number"))) + " Pages";
					//ri.ViewRegisterButton.Click += (sender, e) => { ViewRegister_Click(sender, e, bookNum); };
					ArchivesItemContainer.Items.Add(ri);
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
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

		private void ManualSyncButton_Click(object sender, RoutedEventArgs e)
		{
			SyncArchives();
			SyncRegisters();
		}
	}
}
