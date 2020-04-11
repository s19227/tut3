using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using tut3.Services;

namespace tut3.DAL
{
    public class DbService : IDbService
    {
        private const string CONNECTION_STRING = "Data Source=db-mssql;Initial Catalog=s19227;Integrated Security=True";

        public bool ExistsIndexNumber(string index)
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT COUNT(IndexNumber) AS Count FROM Student WHERE IndexNumber = @index";
                    command.Parameters.AddWithValue("@index", index);

                    connection.Open();

                    var reader = command.ExecuteReader();
                    reader.Read();

                    int result = Convert.ToInt32(reader["Count"]);

                    return result == 1 ? true : false;
                }
            }
        }
    }
}
