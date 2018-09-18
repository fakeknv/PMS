using PMS.UIComponents;
using System.Windows.Controls;
using MahApps.Metro.SimpleChildWindow;
using System.Windows;
using MahApps.Metro.Controls;
using PMS.UIManager.Views.ChildWindows;

namespace PMS.UIManager.Views
{
	/// <summary>
	/// Interaction logic for Tasks.xaml
	/// </summary>
	public partial class Requests : UserControl
	{
		public Requests()
		{
			InitializeComponent();
			//var xx1 = new NotificationItem();
			//xx1.Name = "TestingNotif2";
			//xx1.NotifContent = "Test Notification 2";
			//xx1.NotifDetails = "<data>tchbasdasdasdasditchbitchbitchbitchbitchbitchbitchbitchtchbitchbitchbitchbitchbitchbitchbitchbitch.</data>";
			//xx1.NotifDate = "August 8, 2018";
			//RequestList.Items.Add(xx1);
		}
		private async void CreateRequestButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddRequestWindow(), this.RequestMainGrid);
		}
	}
}