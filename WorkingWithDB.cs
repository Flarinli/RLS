using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System;
using System.Windows.Forms;

namespace RLS
{
    public static class WorkingWithDB
    {
        static string connection_string = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        static SqlCommand sqlCommand;
        static string curID;
        static string experimentID;
        public static string CurID { get { return curID; } }
        public static string ExperimentID { get { return experimentID; } }

        public static string DB_request_string = "";
        public static string selected_table = "";
        public static string selected_column = "";
        public static string uniq_strings = "";
        public static string ordering_string = "";
        public static string where = "";
        public static string comparison = "";
        public static string value = "";
        public static string belonging_to_range = "";
        public static string begin;
        public static string end;
        public static string pattern_matching = "";


        public static async void Insert(string table, List<string> columns, List<string> values)
        {
            SqlConnection sqlConnection = new SqlConnection(connection_string);
            await sqlConnection.OpenAsync();
            string [] arr_columns = columns.ToArray();
            sqlCommand = new SqlCommand($"INSERT INTO { table }({String.Join(", ", arr_columns)}) VALUES(@{String.Join(", @",arr_columns)})", sqlConnection);
            for(int i = 0; i < columns.Count; ++i)
            {
                sqlCommand.Parameters.AddWithValue(columns[i], values[i]);
            }
            await sqlCommand.ExecuteNonQueryAsync();

            SqlDataReader sqlDataReader = null;
            sqlCommand = new SqlCommand($"SELECT @@IDENTITY FROM [{table}]", sqlConnection);
            sqlDataReader = await sqlCommand.ExecuteReaderAsync();
            while (await sqlDataReader.ReadAsync())
            {
                curID = sqlDataReader[0].ToString();
            }
            if (table == "Experiments")
                experimentID = curID;
            sqlCommand.Parameters.Clear();
            sqlConnection.Close();
        }

        public static async void ShowTablesInListBox(ListBox listBox)
        {
            SqlConnection sqlConnection = new SqlConnection(connection_string);
            sqlConnection.Open();

            SqlDataReader sqlDataReader = null;
            sqlCommand = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.TABLES", sqlConnection);
            sqlDataReader = await sqlCommand.ExecuteReaderAsync();
            while (await sqlDataReader.ReadAsync())
            {
                string table = sqlDataReader[2].ToString();
                if(table != "sysdiagrams")
                    listBox.Items.Add(sqlDataReader[2]);
            }
            sqlConnection.Close();
        }

        public static async void ShowColumnsInListBox(string table_name, ListBox listBox)
        {
            SqlConnection sqlConnection = new SqlConnection(connection_string);
            await sqlConnection.OpenAsync();

            SqlDataReader sqlDataReader = null;
            sqlCommand = new SqlCommand($"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{table_name}'", sqlConnection);
            sqlDataReader = await sqlCommand.ExecuteReaderAsync();
            
            listBox.Items.Clear();
            while (await sqlDataReader.ReadAsync())
            {
                string table = sqlDataReader[0].ToString();
                listBox.Items.Add(table);
            }
            sqlConnection.Close();
        }
        
        public static void ClearRequestsData()
        {
            selected_table = "";
            selected_column = "";
            uniq_strings = "";
            ordering_string = "";
            where = "";
            comparison = "";
            value = "";
            belonging_to_range = "";
            pattern_matching = "";
        }

        public static async void SelectRequestToListBox(ListBox listBox)
        {
            DB_request_string = $"SELECT {uniq_strings} {selected_column} FROM {selected_table} {where} {pattern_matching} {comparison} {belonging_to_range}{ordering_string}";
            MessageBox.Show(DB_request_string);
            SqlConnection sqlConnection = new SqlConnection(connection_string);
            await sqlConnection.OpenAsync();
        
            SqlDataReader sqlDataReader = null;
            sqlCommand = new SqlCommand(DB_request_string, sqlConnection);
            sqlDataReader = await sqlCommand.ExecuteReaderAsync();
            listBox.Items.Clear();
            while (await sqlDataReader.ReadAsync())
            {
                string element = sqlDataReader[0].ToString();
                listBox.Items.Add(element);
            }

            sqlConnection.Close();
        }

    }

}
