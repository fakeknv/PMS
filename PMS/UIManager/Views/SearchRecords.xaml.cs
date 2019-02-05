using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using PMS.UIComponents;
using PMS.UIManager.Views.ChildWindows;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace PMS.UIManager.Views
{
    /// <summary>
    /// Interaction logic for Accounts.xaml
    /// </summary>
    public partial class SearchRecords : UserControl
    {

		public SearchRecords()
        {
            InitializeComponent();
		}

		private void SearchRecord_Click(object sender, RoutedEventArgs e)
		{

			if (RegType.Text == "Baptismal")
			{
				SearchBaptismalEntries ce = new SearchBaptismalEntries(SearchRequestBox.Text, RegType.Text, SearchCoverage.SelectedIndex);

				EntriesHolderGrid.Children.Add(ce);
				Grid.SetRow(ce, 0);
				Grid.SetColumn(ce, 0);
			}
			else if (RegType.Text == "Confirmation")
			{
				SearchConfirmationEntries ce = new SearchConfirmationEntries(SearchRequestBox.Text, RegType.Text, SearchCoverage.SelectedIndex);

				EntriesHolderGrid.Children.Add(ce);
				Grid.SetRow(ce, 0);
				Grid.SetColumn(ce, 0);
			}
			else if (RegType.Text == "Matrimonial")
			{
				SearchMatrimonialEntries ce = new SearchMatrimonialEntries(SearchRequestBox.Text, RegType.Text, SearchCoverage.SelectedIndex);

				EntriesHolderGrid.Children.Add(ce);
				Grid.SetRow(ce, 0);
				Grid.SetColumn(ce, 0);
			}
			else if (RegType.Text == "Burial")
			{
				SearchBurialEntries ce = new SearchBurialEntries(SearchRequestBox.Text, RegType.Text, SearchCoverage.SelectedIndex);

				EntriesHolderGrid.Children.Add(ce);
				Grid.SetRow(ce, 0);
				Grid.SetColumn(ce, 0);
			}
		}
	}
}
