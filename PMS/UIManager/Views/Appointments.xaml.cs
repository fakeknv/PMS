using PMS.UIComponents;
using System.Windows.Controls;
using MahApps.Metro.SimpleChildWindow;
using System.Windows;
using MahApps.Metro.Controls;
using PMS.UIManager.Views.ChildWindows;
using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.ObjectModel;

namespace PMS.UIManager.Views
{
    /// <summary>
    /// Interaction logic for Appointments.xaml
    /// </summary>
    public partial class Appointments : UserControl
    {
		//Required for changing the label from another class.
		internal static Appointments app;

		//private MySqlConnection conn;
		//private DBConnectionManager dbman;

		internal string Current_Date
		{
			get { return dayActivitiesTitle.Content.ToString(); }
			set { Dispatcher.Invoke(new Action(() => { dayActivitiesTitle.Content = value; })); }
		}

		public Appointments()
        {
			//Required for changing the label from another class.
			app = this;
            InitializeComponent();
			dayActivitiesTitle.Content = DateTime.Now.ToString("MMMM d, yyyy");
		}

		private async void AddAppointment_Click(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddAppointmentWindow(), this.AppointmentMainGrid);
		}
	}
}
