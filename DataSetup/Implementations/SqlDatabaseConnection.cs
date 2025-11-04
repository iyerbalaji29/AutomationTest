using System.Data;
using System.Data.SqlClient;
using DataSetup.Interfaces;

namespace DataSetup.Implementations
{
    /// <summary>
    /// SQL Server database connection implementation
    /// Follows Single Responsibility Principle - only handles database connections
    /// </summary>
    public class SqlDatabaseConnection : IDatabaseConnection, IDisposable
    {
        private readonly string _connectionString;
        private IDbConnection? _connection;

        public SqlDatabaseConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));

            _connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
            }
            return _connection;
        }

        public void OpenConnection()
        {
            var connection = GetConnection();
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (_connection?.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public void Dispose()
        {
            CloseConnection();
            _connection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
