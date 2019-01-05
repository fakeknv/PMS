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
    public partial class TimeSlots : UserControl
    {
		private MySqlConnection conn;
		private DBConnectionManager dbman;

		private ObservableCollection<TimeSlot> timeslots;
		private ObservableCollection<TimeSlot> timeslots_final;

		public TimeSlots()
        {
            InitializeComponent();
			SyncTimeSlots();
			ItemsPerPage.SelectionChanged += Update2;
			CurrentPage.ValueChanged += Update;
		}
		private void SyncTimeSlots() {
			timeslots = new ObservableCollection<TimeSlot>();
			timeslots_final = new ObservableCollection<TimeSlot>();

			ComboBoxItem ci = (ComboBoxItem)ItemsPerPage.SelectedItem;
			int itemsPerPage = Convert.ToInt32(ci.Content);
			int page = 1;
			int count = 0;

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT * FROM timeslots;";
						MySqlDataReader db_reader = cmd.ExecuteReader();
						while (db_reader.Read())
						{

							timeslots.Add(new TimeSlot()
							{
								TimeSlotID = db_reader.GetString("timeslot_id"),
								Timeslot = DateTime.Parse(db_reader.GetString("timeslot")).ToString("hh:mm tt"),
								Status = db_reader.GetString("status"),
								Page = page
							});
							count++;
							if (count == itemsPerPage)
							{
								page++;
								count = 0;
							}
						}
						foreach (var cur in timeslots)
						{
							if (cur.Page == CurrentPage.Value)
							{
								timeslots_final.Add(new TimeSlot()
								{
									TimeSlotID = cur.TimeSlotID,
									Timeslot = cur.Timeslot,
									Status = cur.Status,
									Page = cur.Page
								});
							}
						}
						//close Connection
						conn2.Close();
						//AccountsItemContainer.Items.Clear();
						TimeslotsItemContainer.Items.Refresh();
						TimeslotsItemContainer.ItemsSource = timeslots_final;
						TimeslotsItemContainer.Items.Refresh();
					}
				}
				else
				{

				}
				conn.Close();
			}
		}
		private void Update(object sender, RoutedPropertyChangedEventArgs<double?> e)
		{
			SyncTimeSlots();
		}
		private void Update2(object sender, SelectionChangedEventArgs e)
		{
			SyncTimeSlots();
		}
		private async void CreateAccountButton_Click(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddTimeSlotWindow());
		}
		private async void MsgNoItemSelected()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "There is no item selected. Please try again.");
		}
		private async void EditButtonClick(object sender, RoutedEventArgs e)
		{
			TimeSlot ts = (TimeSlot)TimeslotsItemContainer.SelectedItem;
			if (ts == null)
			{
				MsgNoItemSelected();
			}
			else
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new EditTimeSlotWindow(ts.TimeSlotID), this.TimeSlotsMainGrid);
			}
		}
	}
}
