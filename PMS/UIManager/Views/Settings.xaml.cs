using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using PMS.UIComponents;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PMS.UIManager.Views
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : UserControl
	{
		private DBConnectionManager dbman;

		public Settings()
		{
			InitializeComponent();
			BaptismalPrintFee.Value = Convert.ToDouble(GetKeyValue("Print Fee Baptismal"));
			MatrimonialPrintFee.Value = Convert.ToDouble(GetKeyValue("Print Fee Matrimonial"));
			BurialPrintFee.Value = Convert.ToDouble(GetKeyValue("Print Fee Burial"));
			ConfirmationPrintFee.Value = Convert.ToDouble(GetKeyValue("Print Fee Confirmation"));

			BaptismalStipend.Value = Convert.ToDouble(GetKeyValue("Baptismal Stipend"));
			ConfirmationStipend.Value = Convert.ToDouble(GetKeyValue("Confirmation Stipend"));
			MatrimonialStipend.Value = Convert.ToDouble(GetKeyValue("Matrimonial Stipend"));
			BurialStipend.Value = Convert.ToDouble(GetKeyValue("Burial Stipend"));
		}
		private string GetKeyValue(string key_name) {
			dbman = new DBConnectionManager();
			string ret = "";

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM settings WHERE key_name = @key_name LIMIT 1;";
				cmd.Parameters.AddWithValue("@key_name", key_name);
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = db_reader.GetString("key_value");
				}
			}
			else
			{
				
			}
			dbman.DBClose();
			return ret;
		}
		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			dbman = new DBConnectionManager();

			double baptismalFee = Convert.ToDouble(BaptismalPrintFee.Value);
			double confirmationFee = Convert.ToDouble(ConfirmationPrintFee.Value);
			double matrimonialFee = Convert.ToDouble(MatrimonialPrintFee.Value);
			double burialFee = Convert.ToDouble(BurialPrintFee.Value);

			double burialStipend = Convert.ToDouble(BurialStipend.Value);
			double matrimonialStipend = Convert.ToDouble(MatrimonialStipend.Value);
			double confirmationStipend = Convert.ToDouble(ConfirmationStipend.Value);
			double baptismalStipend = Convert.ToDouble(BaptismalStipend.Value);

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				//TODO
				try
				{
					string key_name = "Print Fee Baptismal";

					MySqlCommand cmd = dbman.DBConnect().CreateCommand();
					cmd.CommandText =
						"UPDATE settings SET key_value = @key_value WHERE key_name = @key_name;";
					cmd.Parameters.AddWithValue("@key_value", baptismalFee);
					cmd.Parameters.AddWithValue("@key_name", key_name);
					cmd.Prepare();
					int stat_code = cmd.ExecuteNonQuery();
					dbman.DBClose();
					cmd = dbman.DBConnect().CreateCommand();
					key_name = "Print Fee Confirmation";
					cmd.CommandText =
						"UPDATE settings SET key_value = @key_value WHERE key_name = @key_name;";
					cmd.Parameters.AddWithValue("@key_value", confirmationFee);
					cmd.Parameters.AddWithValue("@key_name", key_name);
					cmd.Prepare();
					stat_code = cmd.ExecuteNonQuery();
					dbman.DBClose();
					cmd = dbman.DBConnect().CreateCommand();
					key_name = "Print Fee Matrimonial";
					cmd.CommandText =
						"UPDATE settings SET key_value = @key_value WHERE key_name = @key_name;";
					cmd.Parameters.AddWithValue("@key_value", matrimonialFee);
					cmd.Parameters.AddWithValue("@key_name", key_name);
					cmd.Prepare();
					stat_code = cmd.ExecuteNonQuery();
					dbman.DBClose();
					cmd = dbman.DBConnect().CreateCommand();
					key_name = "Print Fee Burial";
					cmd.CommandText =
						"UPDATE settings SET key_value = @key_value WHERE key_name = @key_name;";
					cmd.Parameters.AddWithValue("@key_value", burialFee);
					cmd.Parameters.AddWithValue("@key_name", key_name);
					cmd.Prepare();
					stat_code = cmd.ExecuteNonQuery();
					dbman.DBClose();
					cmd = dbman.DBConnect().CreateCommand();
					key_name = "Baptismal Stipend";
					cmd.CommandText =
						"UPDATE settings SET key_value = @key_value WHERE key_name = @key_name;";
					cmd.Parameters.AddWithValue("@key_value", baptismalStipend);
					cmd.Parameters.AddWithValue("@key_name", key_name);
					cmd.Prepare();
					stat_code = cmd.ExecuteNonQuery();
					dbman.DBClose();
					cmd = dbman.DBConnect().CreateCommand();
					key_name = "Confirmation Stipend";
					cmd.CommandText =
						"UPDATE settings SET key_value = @key_value WHERE key_name = @key_name;";
					cmd.Parameters.AddWithValue("@key_value", confirmationStipend);
					cmd.Parameters.AddWithValue("@key_name", key_name);
					cmd.Prepare();
					stat_code = cmd.ExecuteNonQuery();
					dbman.DBClose();
					cmd = dbman.DBConnect().CreateCommand();
					key_name = "Matrimonial Stipend";
					cmd.CommandText =
						"UPDATE settings SET key_value = @key_value WHERE key_name = @key_name;";
					cmd.Parameters.AddWithValue("@key_value", matrimonialStipend);
					cmd.Parameters.AddWithValue("@key_name", key_name);
					cmd.Prepare();
					stat_code = cmd.ExecuteNonQuery();
					dbman.DBClose();
					cmd = dbman.DBConnect().CreateCommand();
					key_name = "Burial Stipend";
					cmd.CommandText =
						"UPDATE settings SET key_value = @key_value WHERE key_name = @key_name;";
					cmd.Parameters.AddWithValue("@key_value", burialStipend);
					cmd.Parameters.AddWithValue("@key_name", key_name);
					cmd.Prepare();
					stat_code = cmd.ExecuteNonQuery();
					dbman.DBClose();

					////string tmp = pmsutil.LogRecord(recordID, "LOGC-02");
					SyncValues();
					//InfoArea1.Foreground = new SolidColorBrush(Colors.Green);
					//InfoArea1.Content = "Password successfully changed!";
				}
				catch (MySqlException ex)
				{
					Console.WriteLine("Error: {0}", ex.ToString());
				}
			}
			else {

			}
		}
		private void SyncValues() {
			BaptismalPrintFee.Value = Convert.ToDouble(GetKeyValue("Print Fee Baptismal"));
			MatrimonialPrintFee.Value = Convert.ToDouble(GetKeyValue("Print Fee Matrimonial"));
			BurialPrintFee.Value = Convert.ToDouble(GetKeyValue("Print Fee Burial"));
			ConfirmationPrintFee.Value = Convert.ToDouble(GetKeyValue("Print Fee Confirmation"));

			BaptismalStipend.Value = Convert.ToDouble(GetKeyValue("Baptismal Stipend"));
			ConfirmationStipend.Value = Convert.ToDouble(GetKeyValue("Confirmation Stipend"));
			MatrimonialStipend.Value = Convert.ToDouble(GetKeyValue("Matrimonial Stipend"));
			BurialStipend.Value = Convert.ToDouble(GetKeyValue("Burial Stipend"));
		}
		private void ResetButton_Click(object sender, RoutedEventArgs e)
		{
			SyncValues();
		}
		private async void ActionsHelp_Click(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Actions Help", "You can edit the fees of the transactions in this menu.");
		}
	}
}