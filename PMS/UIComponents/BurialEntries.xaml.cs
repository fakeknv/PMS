using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using PMS.UIManager.Views.ChildWindows;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace PMS.UIComponents
{
    /// <summary>
    /// Interaction logic for ConfirmationEntries.xaml
    /// </summary>
    public partial class BurialEntries : UserControl
    {
		//MYSQL
		private DBConnectionManager dbman;

		private int pnum, bnum;

        public BurialEntries(int bookNum, int pageNum)
        {
			bnum = bookNum;
			pnum = pageNum;
			InitializeComponent();
			SyncBurialEntries(bookNum, pageNum);
		}
		private void SyncBurialEntries(int targBook, int pageNum)
		{
			dbman = new DBConnectionManager();

			EntriesHolder.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM registers WHERE book_number = @book_number LIMIT 1;";
				cmd.Parameters.AddWithValue("@book_number", targBook);
				cmd.Prepare();
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
							BurialRecordEntryItem bre = new BurialRecordEntryItem();
							bre.RecordID.Content = db_reader2.GetString("record_id");
							bre.RegistryNumLabel.Content = db_reader2.GetString("entry_number");
							bre.NameLabel.Text = db_reader2.GetString("recordholder_fullname");
							bre.DeathYearLabel.Content = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy");
							bre.DeathDateLabel.Content = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd");
							bre.BurialYearLabel.Content = "-- Archived --";
							bre.BurialDateLabel.Content = "-- Archived --";
							bre.AgeLabel.Content = "-- Archived --";
							bre.StatusLabel.Text = "-- Archived --";
							bre.ParentSpouse1Label.Text = db_reader2.GetString("parent1_fullname");
							bre.ParentSpouse2Label.Text = db_reader2.GetString("parent2_fullname");
							bre.ResidenceLabel.Text = "-- Archived --";
							bre.SacramentLabel.Text = "-- Archived --";
							bre.CauseOfDeathLabel.Text = "-- Archived --";
							bre.PlaceOfIntermentLabel.Text = "-- Archived --";
							bre.MinisterLabel.Text = "-- Archived --";
							EntriesHolder.Items.Add(bre);
						}
					}
					else
					{
						MySqlCommand cmd2 = dbman.DBConnect().CreateCommand();
						cmd2.CommandText = "SELECT * FROM records, burial_records WHERE records.book_number = @book_number AND records.page_number = @page_number  AND records.record_id = burial_records.record_id ORDER BY records.entry_number ASC;";
						cmd2.Parameters.AddWithValue("@book_number", targBook);
						cmd2.Parameters.AddWithValue("@page_number", pageNum);
						cmd2.Prepare();
						MySqlDataReader db_reader2 = cmd2.ExecuteReader();
						while (db_reader2.Read())
						{
							BurialRecordEntryItem bre = new BurialRecordEntryItem();
							bre.RecordID.Content = db_reader2.GetString("record_id");
							bre.RegistryNumLabel.Content = db_reader2.GetString("entry_number");
							bre.NameLabel.Text = db_reader2.GetString("recordholder_fullname");
							bre.DeathYearLabel.Content = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy");
							bre.DeathDateLabel.Content = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd");
							bre.BurialYearLabel.Content = DateTime.Parse(db_reader2.GetString("burial_date")).ToString("yyyy");
							bre.BurialDateLabel.Content = DateTime.Parse(db_reader2.GetString("burial_date")).ToString("MMM dd");
							bre.AgeLabel.Content = db_reader2.GetString("age");
							bre.StatusLabel.Text = db_reader2.GetString("status");
							bre.ParentSpouse1Label.Text = db_reader2.GetString("parent1_fullname");
							bre.ParentSpouse2Label.Text = db_reader2.GetString("parent2_fullname");
							bre.ResidenceLabel.Text = db_reader2.GetString("residence");
							bre.SacramentLabel.Text = db_reader2.GetString("sacrament");
							bre.CauseOfDeathLabel.Text = db_reader2.GetString("cause_of_death");
							bre.PlaceOfIntermentLabel.Text = db_reader2.GetString("place_of_interment");
							bre.MinisterLabel.Text = db_reader2.GetString("minister");
							EntriesHolder.Items.Add(bre);
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
		private async void MsgNoItemSelected()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "There is no item selected. Please try again.");
		}
		private async void Remarks_Click(object sender, RoutedEventArgs e)
		{
			if (EntriesHolder.SelectedItem == null)
			{
				MsgNoItemSelected();
			}
			else
			{
				BurialRecordEntryItem lvi = (BurialRecordEntryItem)EntriesHolder.SelectedItem;
				Label recordID = (Label)lvi.FindName("RecordID");

				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new ViewRemarksWindow(recordID.Content.ToString()), this.ParentGrid);
			}
		}

		private async void Print_Click(object sender, RoutedEventArgs e)
		{
			if (EntriesHolder.SelectedItem == null)
			{
				MsgNoItemSelected();
			}
			else
			{
				BurialRecordEntryItem lvi = (BurialRecordEntryItem)EntriesHolder.SelectedItem;
				Label recordID = (Label)lvi.FindName("RecordID");

				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new PrintBurialRecordEntryWindow(recordID.Content.ToString()));
			}
		}

		private async void Edit_Click(object sender, RoutedEventArgs e)
		{
			if (EntriesHolder.SelectedItem == null)
			{
				MsgNoItemSelected();
			}
			else
			{
				BurialRecordEntryItem lvi = (BurialRecordEntryItem)EntriesHolder.SelectedItem;
				Label recordID = (Label)lvi.FindName("RecordID");

				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new EditBurialRecordEntryWindow(recordID.Content.ToString()));
			}
		}

		private async void History_Click(object sender, RoutedEventArgs e)
		{
			if (EntriesHolder.SelectedItem == null)
			{
				MsgNoItemSelected();
			}
			else
			{
				BurialRecordEntryItem lvi = (BurialRecordEntryItem)EntriesHolder.SelectedItem;
				Label recordID = (Label)lvi.FindName("RecordID");

				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new ViewHistoryWindow(recordID.Content.ToString()));
			}
		}

		private void UpdateContent(object sender, TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			EntriesHolder.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM registers WHERE book_number = @book_number LIMIT 1;";
				cmd.Parameters.AddWithValue("@book_number", bnum);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (db_reader.GetString("status") == "Archived")
					{
						MySqlCommand cmd2 = dbman.DBConnect().CreateCommand();
						if (SearchFilter.SelectedIndex == 0){
							cmd2.CommandText = "SELECT * FROM records, burial_records WHERE records.book_number = @book_number AND records.record_id = burial_records.record_id AND recordholder_fullname LIKE @query ORDER BY records.entry_number ASC;";
						} else if (SearchFilter.SelectedIndex == 1) {
							cmd2.CommandText = "SELECT * FROM records, burial_records WHERE records.book_number = @book_number AND records.record_id = burial_records.record_id AND records.record_date LIKE @query ORDER BY records.entry_number ASC;";
						} else if (SearchFilter.SelectedIndex == 2) {
							cmd2.CommandText = "SELECT * FROM records, burial_records WHERE records.book_number = @book_number AND records.record_id = burial_records.record_id AND burial_records.burial_date LIKE @query ORDER BY records.entry_number ASC;";
						} else if (SearchFilter.SelectedIndex == 3) {
							cmd2.CommandText = "SELECT * FROM records, burial_records WHERE records.book_number = @book_number AND records.record_id = burial_records.record_id AND (burial_records.status LIKE @query) ORDER BY records.entry_number ASC;";
						} else if (SearchFilter.SelectedIndex == 4) {
							cmd2.CommandText = "SELECT * FROM records, burial_records WHERE records.book_number = @book_number AND records.record_id = burial_records.record_id AND (burial_records.place_of_interment LIKE @query) ORDER BY records.entry_number ASC;";
						} else if (SearchFilter.SelectedIndex == 5) {
							cmd2.CommandText = "SELECT * FROM records, burial_records WHERE records.book_number = @book_number AND records.record_id = burial_records.record_id AND (burial_records.minister LIKE @query) ORDER BY records.entry_number ASC;";
						}
						cmd2.Parameters.AddWithValue("@book_number", bnum);
						cmd2.Parameters.AddWithValue("@query", "%" + SearchBox.Text + "%");
						cmd2.Prepare();
						MySqlDataReader db_reader2 = cmd2.ExecuteReader();
						while (db_reader2.Read())
						{
							BurialRecordEntryItem bre = new BurialRecordEntryItem();
							bre.RecordID.Content = db_reader2.GetString("record_id");
							bre.RegistryNumLabel.Content = db_reader2.GetString("entry_number");
							bre.NameLabel.Text = db_reader2.GetString("recordholder_fullname");
							bre.DeathYearLabel.Content = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy");
							bre.DeathDateLabel.Content = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd");
							bre.BurialYearLabel.Content = "-- Archived --";
							bre.BurialDateLabel.Content = "-- Archived --";
							bre.AgeLabel.Content = "-- Archived --";
							bre.StatusLabel.Text = "-- Archived --";
							bre.ParentSpouse1Label.Text = db_reader2.GetString("parent1_fullname");
							bre.ParentSpouse2Label.Text = db_reader2.GetString("parent2_fullname");
							bre.ResidenceLabel.Text = "-- Archived --";
							bre.SacramentLabel.Text = "-- Archived --";
							bre.CauseOfDeathLabel.Text = "-- Archived --";
							bre.PlaceOfIntermentLabel.Text = "-- Archived --";
							bre.MinisterLabel.Text = "-- Archived --";
							EntriesHolder.Items.Add(bre);
						}
					}
					else
					{
						MySqlCommand cmd2 = dbman.DBConnect().CreateCommand();
						if (SearchFilter.SelectedIndex == 0) {
							cmd2.CommandText = "SELECT * FROM records, burial_records WHERE records.book_number = @book_number AND records.record_id = burial_records.record_id AND records.recordholder_fullname LIKE @query ORDER BY records.entry_number ASC;";
						} else if (SearchFilter.SelectedIndex == 1) {
							cmd2.CommandText = "SELECT * FROM records, burial_records WHERE records.book_number = @book_number AND records.record_id = burial_records.record_id AND records.record_date LIKE @query ORDER BY records.entry_number ASC;";
						} else if (SearchFilter.SelectedIndex == 2) {
							cmd2.CommandText = "SELECT * FROM records, burial_records WHERE records.book_number = @book_number AND records.record_id = burial_records.record_id AND burial_records.burial_date LIKE @query ORDER BY records.entry_number ASC;";
						} else if (SearchFilter.SelectedIndex == 3) {
							cmd2.CommandText = "SELECT * FROM records, burial_records WHERE records.book_number = @book_number AND records.record_id = burial_records.record_id AND (burial_records.status LIKE @query) ORDER BY records.entry_number ASC;";
						} else if (SearchFilter.SelectedIndex == 4) {
							cmd2.CommandText = "SELECT * FROM records, burial_records WHERE records.book_number = @book_number AND records.record_id = burial_records.record_id AND (burial_records.place_of_interment LIKE @query) ORDER BY records.entry_number ASC;";
						} else if (SearchFilter.SelectedIndex == 5) {
							cmd2.CommandText = "SELECT * FROM records, burial_records WHERE records.book_number = @book_number AND records.record_id = burial_records.record_id AND (burial_records.minister LIKE @query) ORDER BY records.entry_number ASC;";
						}
						cmd2.Parameters.AddWithValue("@book_number", bnum);
						cmd2.Parameters.AddWithValue("@query", "%" + SearchBox.Text + "%");
						cmd2.Prepare();
						MySqlDataReader db_reader2 = cmd2.ExecuteReader();
						while (db_reader2.Read())
						{
							BurialRecordEntryItem bre = new BurialRecordEntryItem();
							bre.RecordID.Content = db_reader2.GetString("record_id");
							bre.RegistryNumLabel.Content = db_reader2.GetString("entry_number");
							bre.NameLabel.Text = db_reader2.GetString("recordholder_fullname");
							bre.DeathYearLabel.Content = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy");
							bre.DeathDateLabel.Content = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd");
							bre.BurialYearLabel.Content = DateTime.Parse(db_reader2.GetString("burial_date")).ToString("yyyy");
							bre.BurialDateLabel.Content = DateTime.Parse(db_reader2.GetString("burial_date")).ToString("MMM dd");
							bre.AgeLabel.Content = db_reader2.GetString("age");
							bre.StatusLabel.Text = db_reader2.GetString("status");
							bre.ParentSpouse1Label.Text = db_reader2.GetString("parent1_fullname");
							bre.ParentSpouse2Label.Text = db_reader2.GetString("parent2_fullname");
							bre.ResidenceLabel.Text = db_reader2.GetString("residence");
							bre.SacramentLabel.Text = db_reader2.GetString("sacrament");
							bre.CauseOfDeathLabel.Text = db_reader2.GetString("cause_of_death");
							bre.PlaceOfIntermentLabel.Text = db_reader2.GetString("place_of_interment");
							bre.MinisterLabel.Text = db_reader2.GetString("minister");
							EntriesHolder.Items.Add(bre);
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
	}
}
