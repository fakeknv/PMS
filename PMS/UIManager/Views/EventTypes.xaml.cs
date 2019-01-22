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
    public partial class EventTypes : UserControl
    {
		private MySqlConnection conn;
		private DBConnectionManager dbman;

		private ObservableCollection<EventType> eventtypes;
		private ObservableCollection<EventType> eventtypes_final;

		public EventTypes()
        {
            InitializeComponent();
			SyncEventTypes();
			ItemsPerPage.SelectionChanged += Update2;
			CurrentPage.ValueChanged += Update;
		}
		private void SyncEventTypes() {
			eventtypes = new ObservableCollection<EventType>();
			eventtypes_final = new ObservableCollection<EventType>();

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
						cmd.CommandText = "SELECT * FROM appointment_types;";
						MySqlDataReader db_reader = cmd.ExecuteReader();
						while (db_reader.Read())
						{
							string type = "";
							if (db_reader.GetInt32("custom") == 1) {
								type = "Mass";
							}
							else {
								type = "Special";
							}
							string active = "";
							if (db_reader.GetInt32("status") == 1)
							{
								active = "Active";
							}
							else
							{
								active = "Inactive";
							}
							eventtypes.Add(new EventType()
							{
								TypeID = db_reader.GetString("type_id"),
								AppointmentType = db_reader.GetString("appointment_type"),
								IsCustom = type,
								Fee = db_reader.GetDouble("fee"),
								IsActive = active,
								Page = page
							});
							count++;
							if (count == itemsPerPage)
							{
								page++;
								count = 0;
							}
						}
						foreach (var cur in eventtypes)
						{
							if (cur.Page == CurrentPage.Value)
							{
								eventtypes_final.Add(new EventType()
								{
									TypeID = cur.TypeID,
									AppointmentType = cur.AppointmentType,
									IsCustom = cur.IsCustom,
									Fee = cur.Fee,
									IsActive = cur.IsActive,
									Page = cur.Page
								});
							}
						}
						//close Connection
						conn2.Close();
						//AccountsItemContainer.Items.Clear();
						EventTypeItemContainer.Items.Refresh();
						EventTypeItemContainer.ItemsSource = eventtypes_final;
						EventTypeItemContainer.Items.Refresh();
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
				SyncEventTypes();
			}
			ComboBoxItem ci = (ComboBoxItem)ItemsPerPage.SelectedItem;
			int itemsPerPage = Convert.ToInt32(ci.Content);
			int page = 1;
			int count = 0;

			ObservableCollection<EventType> results = new ObservableCollection<EventType>();
			System.Collections.IList items = EventTypeItemContainer.Items;
			for (int i = 0; i < items.Count - 1; i++)
			{
				EventType item = (EventType)items[i];
				if (item.AppointmentType.Contains(SearchBox.Text) == true || item.IsActive.Contains(SearchBox.Text) == true || item.Fee.ToString().Contains(SearchBox.Text) == true)
				{
					results.Add(new EventType()
					{
						TypeID = item.TypeID,
						AppointmentType = item.AppointmentType,
						IsCustom = item.IsCustom,
						Fee = item.Fee,
						IsActive = item.IsActive,
						Page = item.Page
					});
				}
			}
			EventTypeItemContainer.Items.Refresh();
			EventTypeItemContainer.ItemsSource = results;
			EventTypeItemContainer.Items.Refresh();
			CurrentPage.Maximum = page;
		}
		private void Update(object sender, RoutedPropertyChangedEventArgs<double?> e)
		{
			SyncEventTypes();
		}
		private void Update2(object sender, SelectionChangedEventArgs e)
		{
			SyncEventTypes();
		}
		private async void CreateTypeButton_Click(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddEventTypeWindow());
		}
		private async void MsgNoItemSelected()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "There is no item selected. Please try again.");
		}
		private async void EditButton_Click(object sender, RoutedEventArgs e)
		{
			EventType et = (EventType)EventTypeItemContainer.SelectedItem;
			if (et == null)
			{
				MsgNoItemSelected();
			}
			else
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new EditEventTypeWindow(et.TypeID), this.EventTypesMainGrid);
			}
		}
	}
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            