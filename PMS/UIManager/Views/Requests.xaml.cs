﻿using PMS.UIComponents;
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
			for (int i = 0; i < 15; i++)
			{
				RequestItem ri = new RequestItem();
				RequestItemsContainer.Items.Add(ri);
			}
		}
		private async void CreateRequestButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddRequestWindow(), this.RequestMainGrid);
		}
	}
}