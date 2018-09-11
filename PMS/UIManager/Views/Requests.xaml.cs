using PMS.UIComponents;
using System.Windows.Controls;

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
			var xx1 = new NotificationItem();
			xx1.Name = "TestingNotif2";
			xx1.NotifContent = "Test Notification 2";
			xx1.NotifDetails = "<data>tchbasdasdasdasditchbitchbitchbitchbitchbitchbitchbitchtchbitchbitchbitchbitchbitchbitchbitchbitch.</data>";
			xx1.NotifDate = "August 8, 2018";

			RequestList.Items.Add(xx1);
		}

		private void CreateRequestButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			AddRequestForm addRequestForm = new AddRequestForm();
			addRequestForm.Show();
		}
	}
}