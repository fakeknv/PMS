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
		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public ConfirmBatchPrintWindow(System.Collections.IList items)
		{
			_items = items;
			pmsutil = new PMSUtil();
			InitializeComponent();


			//for (int i = 0; i < items.Count; i++)
			//{
			//	RecordEntryBaptismal recordx = (RecordEntryBaptismal)items[i];
			//	Console.WriteLine(recordx.RecordID);
			//}
		}
		/// <summary>
		/// Interaction logic for the AddRegConfirm button. Gathers and prepares the data
		/// for database insertion.
		/// </summary>
		private async void ConfirmPrint_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new BatchPrintWindow(_items));
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
