using System.Windows.Controls;

namespace PMS.UIComponents
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
        public string NotifDetails {
            get { return NotifContentDetails.Text; }
            set { NotifContentDetails.Text = value; }
        }
        public string NotifDate
        {
            get { return NotifDateHolder.Content.ToString(); }
            set { NotifDateHolder.Content = value; }
        }
    }
}