using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Data;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PMS.UIComponents;
using System.Collections.ObjectModel;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRequestWindow.xaml
	/// </summary>
	public partial class BatchPrintWindow : ChildWindow
	{
		//MYSQL Related Stuff
		DBConnectionManager dbman;

		private PMSUtil pmsutil;


		//private DateTime cDate;
		//private DateTime cTime;
		//private string curDate;
		//private string curTime;

		private ObservableCollection<RecordEntryBaptismal> records;
		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public BatchPrintWindow(System.Collections.IList items)
		{
			pmsutil = new PMSUtil();
			InitializeComponent();

			records = new ObservableCollection<RecordEntryBaptismal>();

			for (int i = 0; i < items.Count; i++)
			{
				RecordEntryBaptismal recordx = (RecordEntryBaptismal)items[i];
				Console.WriteLine(recordx.RecordID);
				records.Add(new RecordEntryBaptismal()
				{
					RecordID = recordx.RecordID,
					EntryNumber = recordx.EntryNumber,
					BaptismalYear = recordx.BaptismalYear,
					BaptismalDate = recordx.BaptismalDate,
					FullName = recordx.FullName,
					BirthDate = recordx.BirthDate,
					Legitimacy = recordx.Legitimacy,
					PlaceOfBirth = recordx.PlaceOfBirth,
					Parent1 = recordx.Parent1,
					Parent2 = recordx.Parent2,
					Godparent1 = recordx.Godparent1,
					Godparent2 = recordx.Godparent2,
					Stipend = recordx.Stipend,
					Minister = recordx.Minister
				});
			}
			EntriesHolder.Items.Refresh();
			EntriesHolder.ItemsSource = records;
			EntriesHolder.Items.Refresh();
		}
		/// <summary>
		/// Interaction logic for the AddRegConfirm button. Gathers and prepares the data
		/// for database insertion.
		/// </summary>
		private void ConfirmPrint_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			//dbman = new DBConnectionManager();
			//if (dbman.DBConnect().State == ConnectionState.Open)
			//{
			//	MySqlCommand cmd = dbman.DBConnect().CreateCommand();
			//	cmd.CommandText = "SELECT * FROM transactions WHERE transaction_id = @transaction_id LIMIT 1;";
			//	cmd.Parameters.AddWithValue("@transaction_id", tid);
			//	cmd.Prepare();
			//	MySqlDataReader db_reader = cmd.ExecuteReader();
			//	while (db_reader.Read())
			//	{
			//		if (db_reader.GetString("status") == "Paying")
			//		{
			//			ornum = OR.Text;

			//			MySqlCommand cmd2 = dbman.DBConnect().CreateCommand();
			//			cmd2.CommandText = "SELECT COUNT(*) FROM transactions WHERE or_number = @or_number;";
			//			cmd2.Parameters.AddWithValue("@or_number", ornum);
			//			cmd2.Prepare();
			//			int counter = int.Parse(cmd2.ExecuteScalar().ToString());
			//			if (counter > 0)
			//			{
			//				InfoArea.Foreground = new SolidColorBrush(Colors.Red);
			//				InfoArea.Content = "Duplicate Receipt Number Detected!";
			//			}
			//			else
			//			{
			//				UpdateTransaction();
			//				trans1.SyncTransactions();
			//				this.Close();
			//			}
			//		}
			//		else if (db_reader.GetString("status") == "Cancelled")
			//		{
			//			MessageBox.Show("Cancelled!");
			//		}
			//		else
			//		{
			//			MessageBox.Show("Already paid!");
			//		}
			//	}
			//}
			//else
			//{

			//}
			//dbman.DBClose();
		}
		/// <summary>
		/// Closes the AddRequestForm Window.
		/// </summary>
		private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
