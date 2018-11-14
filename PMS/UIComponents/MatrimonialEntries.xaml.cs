using MahApps.Metro.Controls;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using PMS.UIManager.Views.ChildWindows;
using System;
using System.Collections.Generic;
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

namespace PMS.UIComponents
{
    /// <summary>
    /// Interaction logic for ConfirmationEntries.xaml
    /// </summary>
    public partial class MatrimonialEntries : UserControl
    {
		//MYSQL
		private DBConnectionManager dbman;

        public MatrimonialEntries(int bookNum, int pageNum)
        {
            InitializeComponent();
			SyncMatrimonialEntries(bookNum, pageNum);
		}
		private void SyncMatrimonialEntries(int targBook, int pageNum)
		{
			dbman = new DBConnectionManager();

			EntriesHolder.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM registers WHERE book_number = @book_number LIMIT 1;";
				cmd.Parameters.AddWithValue("@book_number", targBook);
				cmd.Prepare();
				Console.WriteLine(targBook + " " + pageNum);
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (db_reader.GetString("status") == "Archived")
					{
						MySqlCommand cmd2 = dbman.DBConnect().CreateCommand();
						cmd2.CommandText = "SELECT * FROM records WHERE records.book_number = @book_number AND records.page_number = @page_number ORDER BY records.entry_number ASC;";
						cmd2.Parameters.AddWithValue("@book_number", targBook);
						cmd2.Parameters.AddWithValue("@page_number", pageNum);
						cmd2.Prepare();
						MySqlDataReader db_reader2 = cmd2.ExecuteReader();
						while (db_reader2.Read())
						{
							BaptismalRecordEntryItem bre = new BaptismalRecordEntryItem();
							bre.RecordID.Content = db_reader2.GetString("record_id");
							bre.RegistryNumLabel.Content = db_reader2.GetString("entry_number");
							bre.BaptismalYearLabel.Content = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy");
							bre.BaptismalDateLabel.Content = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd");
							bre.NameLabel.Text = db_reader2.GetString("recordholder_fullname");
							bre.Parent1Label.Text = db_reader2.GetString("parent1_fullname");
							bre.Parent2Label.Text = db_reader2.GetString("parent2_fullname");
							bre.DateOfBirthLabel.Text = "-- Archived --";
							bre.LegitimacyLabel.Content = "-- Archived --";
							bre.Sponsor1Label.Text = "-- Archived --";
							bre.Sponsor2Label.Text = "-- Archived --";
							bre.PlaceOfBirthLabel.Text = "-- Archived --";
							bre.StipendLabel.Content = "0";
							bre.MinisterLabel.Text = "-- Archived --";
							EntriesHolder.Items.Add(bre);
						}
					}
					else
					{
						MySqlCommand cmd2 = dbman.DBConnect().CreateCommand();
						cmd2.CommandText = "SELECT * FROM records, matrimonial_records WHERE records.book_number = @book_number AND records.page_number = @page_number  AND records.record_id = matrimonial_records.record_id ORDER BY records.entry_number ASC;";
						cmd2.Parameters.AddWithValue("@book_number", targBook);
						cmd2.Parameters.AddWithValue("@page_number", pageNum);
						cmd2.Prepare();
						MySqlDataReader db_reader2 = cmd2.ExecuteReader();
						while (db_reader2.Read())
						{
							MatrimonialRecordEntryItem mre = new MatrimonialRecordEntryItem();
							mre.RecordID.Content = db_reader.GetString("record_id");
							mre.EntryNumLabel.Content = db_reader.GetString("entry_number");
							mre.MarriageYearLabel.Content = DateTime.Parse(db_reader.GetString("record_date")).ToString("yyyy");
							mre.MarriageDateLabel.Content = DateTime.Parse(db_reader.GetString("record_date")).ToString("MMM dd");
							mre.Name1Label.Text = db_reader.GetString("recordholder_fullname");
							mre.Name2Label.Text = db_reader.GetString("recordholder2_fullname");
							mre.Status1Label.Content = db_reader.GetString("status1");
							mre.Status2Label.Content = db_reader.GetString("status2");
							mre.Age1Label.Content = db_reader.GetString("age1");
							mre.Age2Label.Content = db_reader.GetString("age2");
							mre.Hometown1Label.Text = db_reader.GetString("place_of_origin1");
							mre.Hometown2Label.Text = db_reader.GetString("place_of_origin2");
							mre.Residence1Label.Text = db_reader.GetString("residence1");
							mre.Residence2Label.Text = db_reader.GetString("residence2");
							mre.Parent1Label1.Text = db_reader.GetString("parent1_fullname");
							mre.Parent2Label1.Text = db_reader.GetString("parent2_fullname");
							mre.Parent1Label2.Text = db_reader.GetString("parent1_fullname2");
							mre.Parent2Label2.Text = db_reader.GetString("parent2_fullname2");
							mre.Witness1Label.Text = db_reader.GetString("witness1");
							mre.Witness2Label.Text = db_reader.GetString("witness2");
							mre.Residence1Label1.Text = db_reader.GetString("witness1address");
							mre.Residence2Label1.Text = db_reader.GetString("witness2address");
							mre.StipendLabel.Text = db_reader.GetString("stipend");
							mre.MinisterLabel.Text = db_reader.GetString("minister");
							EntriesHolder.Items.Add(mre);
						}
					}
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}
		private async void Remarks_Click(object sender, RoutedEventArgs e)
		{
			MatrimonialRecordEntryItem lvi = (MatrimonialRecordEntryItem)EntriesHolder.SelectedItem;
			Label recordID = (Label)lvi.FindName("RecordID");

			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new ViewRemarksWindow(recordID.Content.ToString()), this.ParentGrid);
		}

		private async void Print_Click(object sender, RoutedEventArgs e)
		{
			//MatrimonialRecordEntryItem lvi = (MatrimonialRecordEntryItem)EntriesHolder.SelectedItem;
			//Label recordID = (Label)lvi.FindName("RecordID");

			//var metroWindow = (Application.Current.MainWindow as MetroWindow);
			//await metroWindow.ShowChildWindowAsync(new PrintBaptismalRecordEntryWindow(recordID.Content.ToString()));
		}

		private async void Edit_Click(object sender, RoutedEventArgs e)
		{
			MatrimonialRecordEntryItem lvi = (MatrimonialRecordEntryItem)EntriesHolder.SelectedItem;
			Label recordID = (Label)lvi.FindName("RecordID");

			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new EditMatrimonialRecordEntryWindow(recordID.Content.ToString()));
		}

		private async void History_Click(object sender, RoutedEventArgs e)
		{
			MatrimonialRecordEntryItem lvi = (MatrimonialRecordEntryItem)EntriesHolder.SelectedItem;
			Label recordID = (Label)lvi.FindName("RecordID");

			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new ViewHistoryWindow(recordID.Content.ToString()));
		}
	}
}
