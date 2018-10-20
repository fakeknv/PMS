using MySql.Data.MySqlClient;
using PMS.UIComponents;
using System;
using System.Data;
using System.Windows.Controls;
using System.Windows.Media;

namespace PMS.UIManager.Views
{
	/// <summary>
	/// Interaction logic for Registers.xaml
	/// </summary>
	public partial class Registers : UserControl
	{
		//MYSQL
		DBConnectionManager dbman;

		public Registers()
		{
			InitializeComponent();
			SyncRegisters();
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
		private void SyncRegisters()
		{
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM registers;";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					RegisterItem ri = new RegisterItem();
					ri.BookTypeHolder.Content = db_reader.GetString("book_type");
					ri.BookNoHolder.Content = "Book #" + db_reader.GetString("book_number");
					ri.BookContentStatHolder.Content = CountEntries(Convert.ToInt32(db_reader.GetString("book_number"))) + " Entries | " + CountPages(Convert.ToInt32(db_reader.GetString("book_number"))) + " Pages";
					RegistersContainer.Items.Add(ri);
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}
		private void CreateRequestButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{

		}
	}
}