using MahApps.Metro.Controls;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using PMS.UIComponents;
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

namespace PMS.UIManager.Views.ChildViews
{
    /// <summary>
    /// Interaction logic for ViewRecordEntries.xaml
    /// </summary>
    public partial class ViewRecordEntries : UserControl
    {
		//MYSQL STUFF
		private DBConnectionManager dbman;

		private int booknum;

        public ViewRecordEntries(int bookNum)
        {
			booknum = bookNum;
            InitializeComponent();
			if (GetRegisterType(bookNum) == "Confirmation") {
				ConfirmationEntries ce = new ConfirmationEntries(bookNum, 1);
				EntriesHolderGrid.Children.Add(ce);
				Grid.SetRow(ce, 0);
				Grid.SetColumn(ce, 0);
			}
			else if (GetRegisterType(bookNum) == "Baptismal")
			{
				BaptismalEntries be = new BaptismalEntries(bookNum, 1);
				EntriesHolderGrid.Children.Add(be);
				Grid.SetRow(be, 0);
				Grid.SetColumn(be, 0);
			}
			else if (GetRegisterType(bookNum) == "Matrimonial")
			{
				MatrimonialEntries me = new MatrimonialEntries(bookNum, 1);
				EntriesHolderGrid.Children.Add(me);
				Grid.SetRow(me, 0);
				Grid.SetColumn(me, 0);
			}
			else if (GetRegisterType(bookNum) == "Burial")
			{
				BurialEntries be = new BurialEntries(bookNum, 1);
				EntriesHolderGrid.Children.Add(be);
				Grid.SetRow(be, 0);
				Grid.SetColumn(be, 0);
			}
		}

		private string GetRegisterType(int bookNum)
		{
			string ret = "0";
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM registers WHERE book_number = @book_number;";
				cmd.Parameters.AddWithValue("@book_number", bookNum);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = db_reader.GetString("book_type");
				}
				//close Connection
				dbman.DBClose();
			}
			return ret;
		}

		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			this.Content = new Registers();
		}

		private void UpdatePageContent1(object sender, RoutedPropertyChangedEventArgs<double?> e)
		{
			if (Convert.ToInt32(Page.Value) < 1)
			{
				//Do nothing
			}
			else
			{
				EntriesHolderGrid.Children.Clear();
				if (GetRegisterType(booknum) == "Confirmation") {
					ConfirmationEntries ce = new ConfirmationEntries(booknum, Convert.ToInt32(Page.Value));
					EntriesHolderGrid.Children.Add(ce);
					Grid.SetRow(ce, 0);
					Grid.SetColumn(ce, 0);
				}
				else if(GetRegisterType(booknum) == "Baptismal") {
					BaptismalEntries be = new BaptismalEntries(booknum, Convert.ToInt32(Page.Value));
					EntriesHolderGrid.Children.Add(be);
					Grid.SetRow(be, 0);
					Grid.SetColumn(be, 0);
				}
				else if (GetRegisterType(booknum) == "Matrimonial")
				{
					MatrimonialEntries me = new MatrimonialEntries(booknum, Convert.ToInt32(Page.Value));
					EntriesHolderGrid.Children.Add(me);
					Grid.SetRow(me, 0);
					Grid.SetColumn(me, 0);
				}
				else if (GetRegisterType(booknum) == "Burial")
				{
					BurialEntries be = new BurialEntries(booknum, Convert.ToInt32(Page.Value));
					EntriesHolderGrid.Children.Add(be);
					Grid.SetRow(be, 0);
					Grid.SetColumn(be, 0);
				}
			}
		}

		private async void AddRecord_Click(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddRecordEntryWindow(booknum), this.RecordEntriesMainGrid);
		}
	}
}
