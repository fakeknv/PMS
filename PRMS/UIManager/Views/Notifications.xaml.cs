using System;
using System.Windows.Controls;
using PRMS.UIComponents;

namespace PRMS.UIManager.Views
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
			x1.NotifContent = "TESTd";
			//Main.Items.Add(x1);
		}
	}
}