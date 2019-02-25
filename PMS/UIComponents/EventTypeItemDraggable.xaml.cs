using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PMS.UIComponents
{
	/// <summary>
	/// Interaction logic for EventTypeItemDraggable.xaml
	/// </summary>
	public partial class EventTypeItemDraggable : UserControl
	{
		public string EventName { get; set; }
		public string AppID { get; set; }
		public EventTypeItemDraggable(string _eName, string appID)
		{
			InitializeComponent();
			EventName = _eName;
			EventNameHolder.Content = _eName;
			AppID = appID;
		}
	}
}
