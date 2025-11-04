using System.Data;

namespace DataSetup.Interfaces
{
    /// <summary>
    /// Interface for database connection following Interface Segregation Principle
    /// </summary>
    public interface IDatabaseConnection
    {
        IDbConnection GetConnection();
        void OpenConnection();
        void CloseConnection();
    }
}
