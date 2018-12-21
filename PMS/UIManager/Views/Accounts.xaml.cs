using MahApps.Metro.Controls;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using PMS.UIComponents;
using PMS.UIManager.Views.ChildWindows;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace PMS.UIManager.Views
{
    /// <summary>
    /// Interaction logic for Accounts.xaml
    /// </summary>
    public partial class Accounts : UserControl
    {
		private DBConnectionManager dbman;

		private ObservableCollection<Account> accounts;
		private ObservableCollection<Account> accounts_final;

		private int items;

		public Accounts()
        {
            InitializeComponent();
			items = Convert.ToInt32(ItemsPerPage.Text);
			SyncAccounts();
        }
		private void SyncAccounts() {

			accounts = new ObservableCollection<Account>();
			accounts_final = new ObservableCollection<Account>();

			int itemsPerPage = items;
			int page = 1;
			int count = 0;

			dbman = new DBConnectionManager();

			//AccountsItemContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM accounts, accounts_info WHERE accounts.account_id = accounts_info.account_id;";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					string acc_type;
					switch (db_reader.GetInt32("account_type")) {
						case 1:
							acc_type = "Administrator";
							break;
						case 2:
							acc_type = "Secretary";
							break;
						case 3:
							acc_type = "Registrar";
							break;
						case 4:
							acc_type = "Cashier";
							break;
						case 5:
							acc_type = "Cemetery Staff";
							break;
						case 6:
							acc_type = "Custom";
							break;
						default:
							acc_type = "null";
							break;
					}
					accounts.Add(new Account()
					{
						AccountID = db_reader.GetString("account_id"),
						Username = db_reader.GetString("user_name"),
						Role = acc_type,
						EmpName = db_reader.GetString("name"),
						CreationDate = DateTime.Parse(db_reader.GetString("date_created")).ToString("MMM dd, yyyy"),
						CreationTime = DateTime.Parse(db_reader.GetString("time_created")).ToString("hh:mm tt"),
						Page = page
					});
					count++;
					if (count == itemsPerPage) {
						page++;
						count = 0;
					}
				}
				foreach (var cur in accounts) {
					if (cur.Page == CurrentPage.Value) {
						accounts_final.Add(new Account()
						{
							AccountID = cur.AccountID,
							Username = cur.Username,
							Role = cur.Role,
							EmpName = cur.EmpName,
							CreationDate = cur.CreationDate,
							CreationTime = cur.CreationTime,
							Page = cur.Page
						});
					}
				}
				//close Connection
				dbman.DBClose();
				//AccountsItemContainer.Items.Clear();
				AccountsItemContainer.Items.Refresh();
				AccountsItemContainer.ItemsSource = accounts_final;
				AccountsItemContainer.Items.Refresh();
			}
			else
			{

			}
		}

		private void Update(object sender, RoutedPropertyChangedEventArgs<double?> e)
		{
			SyncAccounts();
		}
		private void Update2(object sender, SelectionChangedEventArgs e)
		{
			SyncAccounts();
		}
		private async void CreateAccountButton_Click(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddAccountWindow());
		}
	}
}
