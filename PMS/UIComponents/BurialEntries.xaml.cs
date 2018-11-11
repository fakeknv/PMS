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
    public partial class BurialEntries : UserControl
    {
		//MYSQL
		private DBConnectionManager dbman;

        public BurialEntries(int bookNum, int pageNum)
        {
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
				cmd.CommandText = "SELECT * FROM records, burial_records WHERE records.book_number = @book_number AND records.page_number = @page_number  AND records.record_id = burial_records.record_id ORDER BY records.entry_number ASC;";
				cmd.Parameters.AddWithValue("@book_number", targBook);
				cmd.Parameters.AddWithValue("@page_number", pageNum);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					BurialRecordEntryItem bre = new BurialRecordEntryItem();
					bre.RecordID.Content = db_reader.GetString("record_id");
					bre.RegistryNumLabel.Content = db_reader.GetString("entry_number");
					bre.NameLabel.Text = db_reader.GetString("recordholder_fullname");
					bre.DeathYearLabel.Content = DateTime.Parse(db_reader.GetString("record_date")).ToString("yyyy");
					bre.DeathDateLabel.Content = DateTime.Parse(db_reader.GetString("record_date")).ToString("MMM dd");
					bre.BurialYearLabel.Content = DateTime.Parse(db_reader.GetString("burial_date")).ToString("yyyy");
					bre.BurialDateLabel.Content = DateTime.Parse(db_reader.GetString("burial_date")).ToString("MMM dd");
					bre.AgeLabel.Content = db_reader.GetString("age");
					bre.StatusLabel.Text = db_reader.GetString("status");
					bre.ParentSpouse1Label.Text = db_reader.GetString("parent1_fullname");
					bre.ParentSpouse2Label.Text = db_reader.GetString("parent2_fullname");
					bre.ResidenceLabel.Text = db_reader.GetString("residence");
					bre.SacramentLabel.Text = db_reader.GetString("sacrament");
					bre.CauseOfDeathLabel.Text = db_reader.GetString("cause_of_death");
					bre.PlaceOfIntermentLabel.Text = db_reader.GetString("place_of_interment");
					bre.MinisterLabel.Text = db_reader.GetString("minister");
					EntriesHolder.Items.Add(bre);
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
			BurialRecordEntryItem lvi = (BurialRecordEntryItem)EntriesHolder.SelectedItem;
			Label recordID = (Label)lvi.FindName("RecordID");

			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new ViewRemarksWindow(recordID.Content.ToString()), this.ParentGrid);
		}

		private async void Print_Click(object sender, RoutedEventArgs e)
		{
			BurialRecordEntryItem lvi = (BurialRecordEntryItem)EntriesHolder.SelectedItem;
			Label recordID = (Label)lvi.FindName("RecordID");

			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new PrintBaptismalRecordEntryWindow(recordID.Content.ToString()));
		}

		private async void Edit_Click(object sender, RoutedEventArgs e)
		{
			BurialRecordEntryItem lvi = (BurialRecordEntryItem)EntriesHolder.SelectedItem;
			Label recordID = (Label)lvi.FindName("RecordID");

			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new EditBurialRecordEntryWindow(recordID.Content.ToString()));
		}

		private async void History_Click(object sender, RoutedEventArgs e)
		{
			BurialRecordEntryItem lvi = (BurialRecordEntryItem)EntriesHolder.SelectedItem;
			Label recordID = (Label)lvi.FindName("RecordID");

			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new ViewHistoryWindow(recordID.Content.ToString()));
		}
	}
}
