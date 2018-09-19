using MahApps.Metro.SimpleChildWindow;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRequestWindow.xaml
	/// </summary>
	public partial class AddRequestWindow : ChildWindow
	{
		public AddRequestWindow()
		{
			InitializeComponent();
		}
		private void AddReqCancel(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
