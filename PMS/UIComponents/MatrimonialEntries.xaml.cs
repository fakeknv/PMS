using MySql.Data.MySqlClient;
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
				cmd.CommandText = "SELECT * FROM records, matrimonial_records WHERE records.book_number = @book_number AND records.page_number = @page_number  AND records.record_id = matrimonial_records.record_id ORDER BY records.entry_number ASC;";
				cmd.Parameters.AddWithValue("@book_number", targBook);
				cmd.Parameters.AddWithValue("@page_number", pageNum);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					MatrimonialRecordEntryItem mre = new MatrimonialRecordEntryItem();
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
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}
	}
}
