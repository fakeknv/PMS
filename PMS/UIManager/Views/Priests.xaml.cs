using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
    public partial class Priests : UserControl
    {
		private MySqlConnection conn;
		private DBConnectionManager dbman;

		private ObservableCollection<Priest> priests;
		private ObservableCollection<Priest> priests_final;

		public Priests()
        {
            InitializeComponent();

			SyncAccounts();
			ItemsPerPage.SelectionChanged += Update2;
			CurrentPage.ValueChanged += Update;
		}
		private void SyncAccounts() {

			priests = new ObservableCollection<Priest>();
			priests_final = new ObservableCollection<Priest>();

			ComboBoxItem ci = (ComboBoxItem)ItemsPerPage.SelectedItem;
			int itemsPerPage = Convert.ToInt32(ci.Content);
			int page = 1;
			int count = 0;

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				//AccountsItemContainer.Items.Clear();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM residing_priests;";
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{

						priests.Add(new Priest()
						{
							PriestID = db_reader.GetString("priest_id"),
							Name = db_reader.GetString("priest_name"),
							Status = db_reader.GetString("priest_status"),
							TotalAServices = CountAssignedServicesTotal(db_reader.GetString("priest_id")),
							Finished = CountAssignedServicesFinished(db_reader.GetString("priest_id")),
							Unfinished = CountAssignedServicesUnfinished(db_reader.GetString("priest_id")),
							Page = page
						});
						count++;
						if (count == itemsPerPage)
						{
							page++;
							count = 0;
						}
					}
					foreach (var cur in priests)
					{
						if (cur.Page == CurrentPage.Value)
						{
							priests_final.Add(new Priest()
							{
								PriestID = cur.PriestID,
								Name = cur.Name,
								Status = cur.Status,
								TotalAServices = cur.TotalAServices,
								Finished = cur.Unfinished,
								Unfinished = cur.Unfinished,
								Page = cur.Page
							});
						}
					}
					//close Connection
					conn.Close();

					PriestsItemContainer.Items.Refresh();
					PriestsItemContainer.ItemsSource = priests_final;
					PriestsItemContainer.Items.Refresh();
					CurrentPage.Maximum = page;
				}
				else
				{

				}
			}
		}
		private int CountAssignedServicesTotal(string pid) {
			int ret = 0;
			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(*) FROM appointments WHERE assigned_priest = @pid;";
					cmd.Parameters.AddWithValue("@pid", pid);
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						ret = db_reader.GetInt32("COUNT(*)");
					}
					//close Connection
					conn.Close();
				}
				else
				{
					ret += 0;
				}
				conn.Close();
			}
			return ret;
		}
		private int CountAssignedServicesFinished(string pid)
		{
			int ret = 0;
			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(*) FROM appointments WHERE assigned_priest = @pid AND status = 2;";
					cmd.Parameters.AddWithValue("@pid", pid);
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						ret = db_reader.GetInt32("COUNT(*)");
					}
					//close Connection
					conn.Close();
				}
				else
				{
					ret += 0;
				}
			}
			return ret;
		}
		private int CountAssignedServicesUnfinished(string pid)
		{
			int ret = 0;
			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = dbman.DBConnect().CreateCommand();
					cmd.CommandText = "SELECT COUNT(*) FROM appointments WHERE assigned_priest = @pid AND status = 1;";
					cmd.Parameters.AddWithValue("@pid", pid);
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						ret = db_reader.GetInt32("COUNT(*)");
					}
					//close Connection
					dbman.DBClose();
				}
				else
				{
					ret += 0;
				}
			}
			return ret;
		}
		private void Update(object sender, RoutedPropertyChangedEventArgs<double?> e)
		{
			SyncAccounts();
		}
		private void Update2(object sender, SelectionChangedEventArgs e)
		{
			SyncAccounts();
		}
		private async void CreatePriestButton_Click(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddPriestWindow());
		}
		private async void MsgNoItemSelected()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "There is no item selected. Please try again.");
		}
		private async void EditPriestButton_Click(object sender, RoutedEventArgs e)
		{
			Priest priest = (Priest)PriestsItemContainer.SelectedItem;
			if (priest == null)
			{
				MsgNoItemSelected();
			}
			else
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new EditPriestWindow(priest.PriestID), this.PriestsMainGrid);
			}
		}
	}
}
