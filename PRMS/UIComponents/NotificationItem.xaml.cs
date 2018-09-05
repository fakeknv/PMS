using System.Windows.Controls;

namespace PRMS.UIComponents
{
	/// <summary>
	/// Interaction logic for Test.xaml
	/// </summary>
	public partial class NotificationItem : UserControl
	{
		public NotificationItem()
		{
			InitializeComponent();
		}
		public string NotifContent{
			get { return NotifContentHolder.Content.ToString(); }
  			set { NotifContentHolder.Content = value; }
		}
	}
}