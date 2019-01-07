using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Data;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PMS.UIComponents;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRequestWindow.xaml
	/// </summary>
	public partial class ConfirmBatchPrintWindow : ChildWindow
	{

		private PMSUtil pmsutil;

		private System.Collections.IList _items;
		private string _type;
		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public ConfirmBatchPrintWindow(System.Collections.IList items, string type)
		{
			_type = type;
			_items = items;
			pmsutil = new PMSUtil();
			InitializeComponent();
		}
		/// <summary>
		/// Interaction logic for the AddRegConfirm button. Gathers and prepares the data
		/// for database insertion.
		/// </summary>
		private async void ConfirmPrint_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (_type == "Baptismal") {
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new BatchPrintWindow(_items));
			}
			else if(_type == "Confirmation") {
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new BatchPrintWindow1(_items));
			}
			else if (_type == "Matrimonial")
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new BatchPrintWindow2(_items));
			}
			else if (_type == "Burial")
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new BatchPrintWindow3(_items));
			}
			this.Close();
		}
		/// <summary>
		/// Closes the AddRequestForm Window.
		/// </summary>
		private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
