using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.data
{
    public class DatabaseConnection
    {
        private readonly string _connectionString = "Server=DESKTOP-KEHORP4;Database=TodoAppDB;Trusted_Connection=True;";

        public async Task<SqlConnection> GetConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            try
            {
                await connection.OpenAsync();
                return connection;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Ошибка подключения к базе данных: {ex.Message}");
                throw;
            }
        }

        public void CloseConnection(SqlConnection connection)
        {
            if (connection != null && connection.State != System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }
    }
}