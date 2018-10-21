using MahApps.Metro.SimpleChildWindow;
using System;
using System.Windows;
using MySql.Data.MySqlClient;
using System.Data;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for ViewRegisterWindow.xaml
	/// </summary>
	public partial class ViewRegisterWindow : ChildWindow
	{
		//MYSQL Related Stuff
		private DBConnectionManager dbman;
		
		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public ViewRegisterWindow(int bookNum)
		{
			InitializeComponent();
			testx.Content = bookNum;
		}
		private void AddReqCancel(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
