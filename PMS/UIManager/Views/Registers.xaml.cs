using MahApps.Metro.Controls;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using PMS.UIComponents;
using PMS.UIManager.Views.ChildViews;
using PMS.UIManager.Views.ChildWindows;
using System;
using System.Data;
using System.Windows;
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
					RegisterItem ri = new RegisterItem();
					ri.BookTypeHolder.Content = db_reader.GetString("book_type");
					ri.BookNoHolder.Content = "Book #" + db_reader.GetString("book_number");
					ri.BookContentStatHolder.Content = CountEntries(Convert.ToInt32(db_reader.GetString("book_number"))) + " Entries | " + CountPages(Convert.ToInt32(db_reader.GetString("book_number"))) + " Pages";
					ri.ViewRegisterButton.Click += (sender, e) => { ViewRegister_Click(sender, e, bookNum); };
					RegistersItemContainer.Items.Add(ri);
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}
		/// <summary>
		/// Onclick event for the Baptismal Filter Button. Filters the Requests to show
		/// registers of type of Bapismal.
		/// </summary>
		private void ShowBaptismal_Click(object sender, RoutedEventArgs e)
		{
			dbman = new DBConnectionManager();

			RegistersItemContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM registers WHERE book_type = @book_type;";
				cmd.Parameters.AddWithValue("@book_type", "Baptismal");
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					RegisterItem ri = new RegisterItem();
					ri.BookTypeHolder.Content = db_reader.GetString("book_type");
					ri.BookNoHolder.Content = "Book #" + db_reader.GetString("book_number");
					ri.BookContentStatHolder.Content = CountEntries(Convert.ToInt32(db_reader.GetString("book_number"))) + " Entries | " + CountPages(Convert.ToInt32(db_reader.GetString("book_number"))) + " Pages";
					RegistersItemContainer.Items.Add(ri);
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}
		/// <summary>
		/// Onclick event for the Confirmation Filter Button. Filters the Requests to show
		/// registers of type of Confirmation.
		/// </summary>
		private void ShowConfirmation_Click(object sender, RoutedEventArgs e)
		{
			dbman = new DBConnectionManager();

			RegistersItemContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM registers WHERE book_type = @book_type;";
				cmd.Parameters.AddWithValue("@book_type", "Confirmation");
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					RegisterItem ri = new RegisterItem();
					ri.BookTypeHolder.Content = db_reader.GetString("book_type");
					ri.BookNoHolder.Content = "Book #" + db_reader.GetString("book_number");
					ri.BookContentStatHolder.Content = CountEntries(Convert.ToInt32(db_reader.GetString("book_number"))) + " Entries | " + CountPages(Convert.ToInt32(db_reader.GetString("book_number"))) + " Pages";
					RegistersItemContainer.Items.Add(ri);
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}
		/// <summary>
		/// Onclick event for the Matrimonial Filter Button. Filters the Requests to show
		/// registers of type of Matrimonial.
		/// </summary>
		private void ShowMatrimonial_Click(object sender, RoutedEventArgs e)
		{
			dbman = new DBConnectionManager();

			RegistersItemContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM registers WHERE book_type = @book_type;";
				cmd.Parameters.AddWithValue("@book_type", "Matrimonial");
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					RegisterItem ri = new RegisterItem();
					ri.BookTypeHolder.Content = db_reader.GetString("book_type");
					ri.BookNoHolder.Content = "Book #" + db_reader.GetString("book_number");
					ri.BookContentStatHolder.Content = CountEntries(Convert.ToInt32(db_reader.GetString("book_number"))) + " Entries | " + CountPages(Convert.ToInt32(db_reader.GetString("book_number"))) + " Pages";
					RegistersItemContainer.Items.Add(ri);
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}
		/// <summary>
		/// Onclick event for the Burial Filter Button. Filters the Requests to show
		/// registers of type of Burial.
		/// </summary>
		private void ShowBurial_Click(object sender, RoutedEventArgs e)
		{
			dbman = new DBConnectionManager();

			RegistersItemContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM registers WHERE book_type = @book_type;";
				cmd.Parameters.AddWithValue("@book_type", "Burial");
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					RegisterItem ri = new RegisterItem();
					ri.BookTypeHolder.Content = db_reader.GetString("book_type");
					ri.BookNoHolder.Content = "Book #" + db_reader.GetString("book_number");
					ri.BookContentStatHolder.Content = CountEntries(Convert.ToInt32(db_reader.GetString("book_number"))) + " Entries | " + CountPages(Convert.ToInt32(db_reader.GetString("book_number"))) + " Pages";
					RegistersItemContainer.Items.Add(ri);
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

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
		/// Onclick event for the AddRegisterWindow. Shows the AddRegisterWindow that allows 
		/// the user to add a register.
		/// </summary>
		private async void CreateRegisterButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddRegisterWindow(this), this.RegisterMainGrid);
		}
		/// <summary>
		/// Onclick event for the ViewRegisterButton. Shows the ViewRegisterWindow that allows 
		/// the user to add a register.
		/// </summary>
		private void ViewRegister_Click(object sender, EventArgs e, int bookNum)
		{
			// set the content
			this.Content = new ViewRecordEntries(bookNum);
		}
	}
}