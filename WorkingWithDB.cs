using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System;
using System.Windows.Forms;

namespace RLS
{
    public static class WorkingWithDB
    {

        static SqlConnection sqlConnection;
        static string connection_string = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        static SqlCommand sqlCommand;
        static string curID;
        static string experimentID;
        public static string CurID { get { return curID; } }
        public static string ExperimentID { get { return experimentID; } }

        public static async void Insert(string table, List<string> columns, List<string> values)
        {
            sqlConnection = new SqlConnection(connection_string);
            sqlConnection.Open();
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
            MessageBox.Show(CurID);//УБРАТЬ ПОТОМ
            sqlCommand.Parameters.Clear();
            sqlConnection.Close();
        }
      
    }

}
