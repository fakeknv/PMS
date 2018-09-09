using System;
using System.Windows.Controls;
using PMS.UIComponents;

namespace PMS.UIManager.Views
{
	/// <summary>
	/// Interaction logic for Notifications.xaml
	/// </summary>
	public partial class Notifications : UserControl
	{
		public Notifications()
		{
			InitializeComponent();
			NotificationItem x1 = new NotificationItem();
			x1.Name = "TestingNotif";
			x1.NotifContent = "Test Notification 1";

            var listBox = new ListBox();
            NotifContainer.Content = listBox;
            listBox.Items.Add(x1);
            NotificationItem x2 = new NotificationItem();
            x2.Name = "TestingNotif2";
            x2.NotifContent = "Test Notification 2";
            listBox.Items.Add(x2);
            //Main.Items.Add(x1);
            FilterButton.Items.Add("asd");
        }

    }
}