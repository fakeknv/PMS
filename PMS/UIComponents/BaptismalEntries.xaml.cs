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
    public partial class BaptismalEntries : UserControl
    {
		//MYSQL
		private DBConnectionManager dbman;

        public BaptismalEntries(int bookNum, int pageNum)
        {
            InitializeComponent();
			SyncConfirmationEntries(bookNum, pageNum);
		}
		private void SyncConfirmationEntries(int targBook, int pageNum)
		{
			dbman = new DBConnectionManager();

			EntriesHolder.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM records, baptismal_records WHERE records.book_number = @book_number AND records.page_number = @page_number  AND records.record_id = baptismal_records.record_id ORDER BY records.entry_number ASC;";
				cmd.Parameters.AddWithValue("@book_number", targBook);
				cmd.Parameters.AddWithValue("@page_number", pageNum);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					BaptismalRecordEntryItem bre = new BaptismalRecordEntryItem();
					bre.RegistryNumLabel.Content = db_reader.GetString("entry_number");
					bre.BaptismalYearLabel.Content = DateTime.Parse(db_reader.GetString("record_date")).ToString("yyyy");
					bre.BaptismalDateLabel.Content = DateTime.Parse(db_reader.GetString("record_date")).ToString("MMM dd");
					bre.NameLabel.Text = db_reader.GetString("recordholder_fullname");
					bre.Parent1Label.Text = db_reader.GetString("parent1_fullname");
					bre.Parent2Label.Text = db_reader.GetString("parent2_fullname");
					bre.DateOfBirthLabel.Text = DateTime.Parse(db_reader.GetString("birthday")).ToString("MMM dd, yyyy");
					bre.LegitimacyLabel.Content = db_reader.GetString("legitimacy");
					bre.Sponsor1Label.Text = db_reader.GetString("sponsor1");
					bre.Sponsor2Label.Text = db_reader.GetString("sponsor2");
					bre.PlaceOfBirthLabel.Text = db_reader.GetString("place_of_birth");
					bre.StipendLabel.Content = db_reader.GetInt32("stipend");
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
	}
}
