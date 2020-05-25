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
        private static readonly List<string> types_of_requests = new List<string>() { "Сравнение", "Попадание в диапазон", "Соответствие шаблону"};


        public static string selected_table = "";
        public static List<string> table_columns;
        public static List<string> selected_columns = new List<string>();
        public static string selected_cond_column = "";


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

        public static void ShowTablesInListBox(ListBox listBox)
        {
            SqlConnection sqlConnection = new SqlConnection(connection_string);
            sqlConnection.Open();

            SqlDataReader sqlDataReader = null;
            sqlCommand = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.TABLES", sqlConnection);
            sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                string table = sqlDataReader[2].ToString();
                if(table != "sysdiagrams")
                    listBox.Items.Add(sqlDataReader[2]);
            }
            sqlConnection.Close();
        }

        public static void GetTableColumns()
        {
            SqlConnection sqlConnection = new SqlConnection(connection_string);
            sqlConnection.Open();

            SqlDataReader sqlDataReader = null;
            sqlCommand = new SqlCommand($"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{selected_table}'", sqlConnection);
            sqlDataReader = sqlCommand.ExecuteReader();
            table_columns = new List<string>();
            while (sqlDataReader.Read())
            {
                table_columns.Add(sqlDataReader[0].ToString());
            }
            sqlConnection.Close();
        }

        public static void ShowColumnsInListBox(ListBox listBox)
        {
            listBox.Items.Clear();
            foreach (string column in table_columns) listBox.Items.Add(column);
        }

        public static void ShowColumnsInCheckedListBox(CheckedListBox checkedList)
        {
            checkedList.Items.Clear();
            foreach(string column in table_columns) checkedList.Items.Add(column);
        }
 
        public static void ShowColumnsAndRequestsInDataGrid(DataGridView dataGridView, int row)
        {   
            DataGridViewComboBoxCell dcombo = new DataGridViewComboBoxCell();
            foreach(string column in table_columns) dcombo.Items.Add(column);
            dataGridView.Rows[row].Cells[0] = dcombo;

            dcombo = new DataGridViewComboBoxCell();
            foreach(string type in types_of_requests) dcombo.Items.Add(type);
            dataGridView.Rows[row].Cells[1] = dcombo;
        }

            public static void ClearRequestsData()
        {
            selected_table = "";
            selected_columns.Clear();
            selected_cond_column = "";
        }


        public static void FinalRequest(DataGridView dataGrid, DataGridView outDataGrid)
        {
            string req_str =  $"SELECT {string.Join(",", selected_columns.ToArray())} FROM {selected_table} ";
            if (dataGrid.Rows.Count != 1)
                req_str += "WHERE\n";
            for (int i = 0; i < (dataGrid.Rows.Count - 1); ++i)
            {
                string column = dataGrid.Rows[i].Cells[0].Value.ToString();
                string condition = dataGrid.Rows[i].Cells[1].Value.ToString();
                string value = dataGrid.Rows[i].Cells[2].Value.ToString();
                switch (condition)
                {                        
                    case "Сравнение":
                        {
                            if (!(value.Contains("=") || value.Contains(">") || value.Contains("<") || value.Contains(">=") || value.Contains("<=") || value.Contains("<>")))
                                break;
                            req_str += $"{column} {value} ";
                            break;
                        }
                        
                    case "Попадание в диапазон":
                        {
                            string [] range = value.Split(' ');
                            req_str += $"{column} BETWEEN {range[0]} AND {range[1]} ";
                            break;
                        }
                    case "Соответствие шаблону":
                        {
                            if (!(value.Contains("%") || value.Contains("_") || value.Contains("[]") || value.Contains("[^]")))
                                break;
                            req_str += $"{column} LIKE '{value}' ";
                            break;
                        }
                }
                if (i != dataGrid.Rows.Count - 2)
                    req_str += "\n AND ";
            }

            SqlConnection sqlConnection = new SqlConnection(connection_string);
            sqlConnection.Open();

            SqlDataReader sqlDataReader = null;
            sqlCommand = new SqlCommand(req_str, sqlConnection);
            sqlDataReader = sqlCommand.ExecuteReader();

            outDataGrid.Rows.Clear();
            outDataGrid.Columns.Clear();
            for (int i = 0; i < selected_columns.Count; ++i)
            {
                outDataGrid.Columns.Add(selected_columns[i], selected_columns[i]);
                outDataGrid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            int row_number = 0;
            while (sqlDataReader.Read())
            {
                outDataGrid.Rows.Add();
                for (int i = 0; i < sqlDataReader.FieldCount; ++i)
                {
                    outDataGrid[i, row_number].Value = sqlDataReader[i].ToString();
                }
                ++row_number;
            }
            ClearRequestsData();
            sqlConnection.Close();
        }
    }

}
