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
						CurrentPage.Maximum = page;
					}
				}
				else
				{

				}
				conn.Close();
			}
		}
		private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (string.IsNullOrEmpty(SearchBox.Text))
			{
				SyncTimeSlots();
			}
			ComboBoxItem ci = (ComboBoxItem)ItemsPerPage.SelectedItem;
			int itemsPerPage = Convert.ToInt32(ci.Content);
			int page = 1;
			int count = 0;

			ObservableCollection<TimeSlot> results = new ObservableCollection<TimeSlot>();
			System.Collections.IList items = TimeslotsItemContainer.Items;
			for (int i = 0; i < items.Count - 1; i++)
			{
				TimeSlot item = (TimeSlot)items[i];
				if (item.Timeslot.Contains(SearchBox.Text) == true || item.Status.Contains(SearchBox.Text) == true)
				{
					results.Add(new TimeSlot()
					{
						TimeSlotID = item.TimeSlotID,
						Timeslot = item.Timeslot,
						Status = item.Status,
						Page = item.Page
					});
				}
			}
			TimeslotsItemContainer.Items.Refresh();
			TimeslotsItemContainer.ItemsSource = results;
			TimeslotsItemContainer.Items.Refresh();
			CurrentPage.Maximum = page;
		}
		private void Update(object sender, RoutedPropertyChangedEventArgs<double?> e)
		{
			SyncTimeSlots();
		}
		private void Update2(object sender, SelectionChangedEventArgs e)
		{
			SyncTimeSlots();
		}
		private async void CreateButton_Click(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddTimeSlotWindow());
		}
		private async void MsgNoItemSelected()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "There is no item selected. Please try again.");
		}
		private async void EditButton_Click(object sender, RoutedEventArgs e)
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
