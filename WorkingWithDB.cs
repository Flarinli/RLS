using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System;
using System.Windows.Forms;
using System.Linq;
using System.CodeDom.Compiler;

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
        public static List<string> joining_tables = new List<string>();
        public static List<string> joining_columns = new List<string>();
        public static List<string> table_columns;
        public static List<string> selected_columns = new List<string>();
        public static string selected_cond_column = "";
        private static List<string> selected_types_of_join = new List<string>();
        private static readonly List<string> types_of_join = new List<string>() {"Join On", "Left Join On", "Right Join On"};
        private static string first_join_word = "";
        private static string second_join_word = "";


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

        public static void ShowTablesInDataGridView(DataGridView dataGridView, int row)
        {
            SqlConnection sqlConnection = new SqlConnection(connection_string);
            sqlConnection.Open();

            SqlDataReader sqlDataReader = null;
            sqlCommand = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.TABLES", sqlConnection);
            sqlDataReader = sqlCommand.ExecuteReader();

            DataGridViewComboBoxCell dcombo = new DataGridViewComboBoxCell();
            while (sqlDataReader.Read())
            {
                string table = sqlDataReader[2].ToString();
                if(table != "sysdiagrams")
                    dcombo.Items.Add(sqlDataReader[2]);
            }
            sqlConnection.Close();
            dataGridView.Rows[row].Cells[0] = dcombo;
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

        public static void UpdateDataGridRow(DataGridView dataGridView, int row)
        {
            selected_table = dataGridView.Rows[row].Cells[0].Value.ToString();

            GetTableColumns();

            DataGridViewComboBoxCell dcombo;

            dcombo = new DataGridViewComboBoxCell();
            foreach (string column in table_columns) dcombo.Items.Add(column);
            dataGridView.Rows[row].Cells[1] = dcombo;

            dcombo = new DataGridViewComboBoxCell();
            foreach (string type in types_of_join) dcombo.Items.Add(type);
            dataGridView.Rows[row].Cells[2] = dcombo;

            dcombo = new DataGridViewComboBoxCell();
            foreach (string column in table_columns) dcombo.Items.Add(column);
            dataGridView.Rows[row].Cells[3] = dcombo;

            dcombo = new DataGridViewComboBoxCell();
            foreach (string type in types_of_requests) dcombo.Items.Add(type);
            dataGridView.Rows[row].Cells[4] = dcombo;
        }

        public static void ClearRequestsData()
        {
            selected_table = "";
            joining_tables.Clear();
            joining_columns.Clear();
            selected_columns.Clear();
            selected_cond_column = "";
        }


        public static void FinalRequest(DataGridView dataGrid, DataGridView outDataGrid)
        {
            for (int i = 0; i < dataGrid.Rows.Count - 1; ++i)
            {
                if (dataGrid.Rows[i].Cells[0].Value != null)
                {
                    joining_tables.Add(dataGrid.Rows[i].Cells[0].Value.ToString());
                }

                if (dataGrid.Rows[i].Cells[1].Value != null)
                {
                    joining_columns.Add(dataGrid.Rows[i].Cells[1].Value.ToString());
                }
                if (dataGrid.Rows[i].Cells[2].Value != null)
                    selected_types_of_join.Add(dataGrid.Rows[i].Cells[2].Value.ToString());
          
                if (dataGrid.Rows[i].Cells[3].Value != null)
                    selected_columns.Add(dataGrid.Rows[i].Cells[3].Value.ToString());
            }

            string req_str = "";
            if (joining_tables.Count > 1)
            {

                for (int i = 0, j = 0; i < joining_tables.Count(); ++i, ++j)
                {

                    string type = selected_types_of_join[j];
                    switch (type)
                    {
                        case "Join On":
                            {
                                first_join_word = "JOIN";
                                second_join_word = "ON";
                                break;
                            }
                        case "Left Join On":
                            {
                                first_join_word = "LEFT JOIN";
                                second_join_word = "ON";
                                break;
                            }

                        case "Right Join On":
                            {
                                first_join_word = "RIGHT JOIN";
                                second_join_word = "ON";
                                break;
                            }
                    }
                    if (i == 0)
                    {
                        req_str += "SELECT ";
                        for (int c = 0; c < joining_tables.Count; ++c)
                        {
                            req_str += $"{joining_tables[c]}.{selected_columns[c]}";
                            if (c != joining_columns.Count - 1)
                                req_str += ", ";
                            else
                                req_str += " ";
                        }
                        req_str += $"FROM {joining_tables[i]} {first_join_word} {joining_tables[i + 1]} {second_join_word} ({joining_tables[i]}.{joining_columns[i]} = {joining_tables[i + 1]}.{joining_columns[i + 1]}) ";
                    }
                    else if (i != joining_tables.Count-1)
                    {
                        req_str += $"{first_join_word} {joining_tables[i]} {second_join_word} ({joining_tables[i - 1]}.{joining_columns[i - 1]} = {joining_tables[i]}.{joining_columns[i]}) ";
                    }
                        
                }
            }
            else
            {
                req_str = $"SELECT {string.Join(",", selected_columns.ToArray())} FROM {joining_tables[0]} ";
            }
            if (dataGrid.Rows.Count != 1)
                req_str += "WHERE\n";
            for (int i = 0; i < (dataGrid.Rows.Count - 1); ++i)
            {
                string column = "";
                string condition = "";
                string value = "";
                if (dataGrid.Rows[i].Cells[3].Value != null)
                    column = dataGrid.Rows[i].Cells[3].Value.ToString();
                if (dataGrid.Rows[i].Cells[4].Value != null)
                    condition = dataGrid.Rows[i].Cells[4].Value.ToString();
                if (dataGrid.Rows[i].Cells[5].Value != null)
                    value = dataGrid.Rows[i].Cells[5].Value.ToString();
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
            MessageBox.Show(req_str);

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
