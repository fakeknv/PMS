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
			/*RequestItemsGrid.ColumnDefinitions.Add(new ColumnDefinition());
			RequestItemsGrid.ColumnDefinitions.Add(new ColumnDefinition());
			RequestItemsGrid.ColumnDefinitions.Add(new ColumnDefinition());


			Grid.SetColumn(new RequestItem(), 0);
			RequestItemsGrid.Children.Add(new RequestItem());
			Grid.SetColumn(new RequestItem(), 1);
			RequestItemsGrid.Children.Add(new RequestItem());
			Grid.SetColumn(new RequestItem(), 2);
			RequestItemsGrid.Children.Add(new RequestItem());*/

		}
		private async void CreateRequestButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddRequestWindow(), this.RequestMainGrid);
		}
	}
}