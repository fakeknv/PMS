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
    public partial class ConfirmationEntries : UserControl
    {
		//MYSQL
		private DBConnectionManager dbman;

        public ConfirmationEntries(int bookNum, int pageNum)
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
				cmd.CommandText = "SELECT * FROM records, confirmation_records WHERE records.book_number = @book_number AND records.page_number = @page_number  AND records.record_id = confirmation_records.record_id ORDER BY records.entry_number ASC;";
				cmd.Parameters.AddWithValue("@book_number", targBook);
				cmd.Parameters.AddWithValue("@page_number", pageNum);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ConfirmationRecordEntryItem cre = new ConfirmationRecordEntryItem();
					cre.RegistryNumLabel.Content = db_reader.GetString("entry_number");
					cre.ConfirmationYearLabel.Content = DateTime.Parse(db_reader.GetString("record_date")).ToString("yyyy");
					cre.ConfirmationDateLabel.Content = DateTime.Parse(db_reader.GetString("record_date")).ToString("MMM dd");
					cre.NameLabel.Text = db_reader.GetString("recordholder_fullname");
					cre.Parent1Label.Text = db_reader.GetString("parent1_fullname");
					cre.Parent2Label.Text = db_reader.GetString("parent2_fullname");
					cre.AgeLabel.Content = db_reader.GetString("age");
					cre.ParishLabel.Content = db_reader.GetString("parochia");
					cre.ProvinceLabel.Content = db_reader.GetString("province");
					cre.PlaceOfBaptismLabel.Text = db_reader.GetString("place_of_baptism");
					cre.Sponsor1Label.Text = db_reader.GetString("sponsor");
					cre.StipendLabel.Content = db_reader.GetString("stipend");
					cre.MinisterLabel.Text = db_reader.GetString("minister");
					EntriesHolder.Items.Add(cre);
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
