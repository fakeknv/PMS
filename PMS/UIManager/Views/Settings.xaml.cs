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

		private PMSUtil pmsutil;

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

			Thanksgiving.Value = Convert.ToDouble(GetKeyValue("Thanksgiving Mass Fee"));
			PetitionMass.Value = Convert.ToDouble(GetKeyValue("Petition Mass Fee"));
			SpecialIntentionMass.Value = Convert.ToDouble(GetKeyValue("Special Intention Fee"));
			AllSouls.Value = Convert.ToDouble(GetKeyValue("All Souls Fee"));
			Souls.Value = Convert.ToDouble(GetKeyValue("Soul/s of Fee"));

			FetchAccounts();
		}
		private void FetchAccounts() {
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM accounts, accounts_info WHERE accounts.account_id = accounts_info.account_id;";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					AccountsItem ai = new AccountsItem();
					ai.Username.Content = db_reader.GetString("user_name");
					ai.AccountType.Content = pmsutil.GetAccountType(db_reader.GetString("account_id"));
					ai.FullName.Content = db_reader.GetString("name");
					AccountsListHolder.Items.Add(ai);
				}
			}
			else
			{

			}
			dbman.DBClose();
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

			double thanksgivingFee = Convert.ToDouble(Thanksgiving.Value);
			double petitionFee = Convert.ToDouble(PetitionMass.Value);
			double specialIntentionFee = Convert.ToDouble(SpecialIntentionMass.Value);
			double allSoulsFee = Convert.ToDouble(AllSouls.Value);
			double soulsFee = Convert.ToDouble(Souls.Value);

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
					key_name = "Thanksgiving Mass Fee";
					cmd.CommandText =
						"UPDATE settings SET key_value = @key_value WHERE key_name = @key_name;";
					cmd.Parameters.AddWithValue("@key_value", thanksgivingFee);
					cmd.Parameters.AddWithValue("@key_name", key_name);
					cmd.Prepare();
					stat_code = cmd.ExecuteNonQuery();
					dbman.DBClose();
					cmd = dbman.DBConnect().CreateCommand();
					key_name = "Petition Mass Fee";
					cmd.CommandText =
						"UPDATE settings SET key_value = @key_value WHERE key_name = @key_name;";
					cmd.Parameters.AddWithValue("@key_value", petitionFee);
					cmd.Parameters.AddWithValue("@key_name", key_name);
					cmd.Prepare();
					stat_code = cmd.ExecuteNonQuery();
					dbman.DBClose();
					cmd = dbman.DBConnect().CreateCommand();
					key_name = "Special Intention Fee";
					cmd.CommandText =
						"UPDATE settings SET key_value = @key_value WHERE key_name = @key_name;";
					cmd.Parameters.AddWithValue("@key_value", specialIntentionFee);
					cmd.Parameters.AddWithValue("@key_name", key_name);
					cmd.Prepare();
					stat_code = cmd.ExecuteNonQuery();
					dbman.DBClose();
					cmd = dbman.DBConnect().CreateCommand();
					key_name = "All Souls Fee";
					cmd.CommandText =
						"UPDATE settings SET key_value = @key_value WHERE key_name = @key_name;";
					cmd.Parameters.AddWithValue("@key_value", allSoulsFee);
					cmd.Parameters.AddWithValue("@key_name", key_name);
					cmd.Prepare();
					stat_code = cmd.ExecuteNonQuery();
					dbman.DBClose();
					cmd = dbman.DBConnect().CreateCommand();
					key_name = "Soul/s of Fee";
					cmd.CommandText =
						"UPDATE settings SET key_value = @key_value WHERE key_name = @key_name;";
					cmd.Parameters.AddWithValue("@key_value", soulsFee);
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

			Thanksgiving.Value = Convert.ToDouble(GetKeyValue("Thanksgiving Mass Fee"));
			PetitionMass.Value = Convert.ToDouble(GetKeyValue("Petition Mass Fee"));
			SpecialIntentionMass.Value = Convert.ToDouble(GetKeyValue("Special Intention Fee"));
			AllSouls.Value = Convert.ToDouble(GetKeyValue("All Souls Fee"));
			Souls.Value = Convert.ToDouble(GetKeyValue("Soul/s of Fee"));
		}
		private void ResetButton_Click(object sender, RoutedEventArgs e)
		{
			SyncValues();
		}
	}
}