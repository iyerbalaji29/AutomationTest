using System.Data;
using DataSetup.Interfaces;

namespace DataSetup.Implementations
{
    /// <summary>
    /// SQL Script executor implementation
    /// Follows Dependency Inversion Principle - depends on IDatabaseConnection abstraction
    /// </summary>
    public class SqlScriptExecutor : ISqlScriptExecutor
    {
        private readonly IDatabaseConnection _databaseConnection;

        public SqlScriptExecutor(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection ?? throw new ArgumentNullException(nameof(databaseConnection));
        }

        public void ExecuteScript(string scriptPath)
        {
            if (!File.Exists(scriptPath))
                throw new FileNotFoundException($"SQL script not found: {scriptPath}");

            var scriptContent = File.ReadAllText(scriptPath);
            ExecuteScriptContent(scriptContent);
        }

        public void ExecuteScriptContent(string scriptContent)
        {
            if (string.IsNullOrWhiteSpace(scriptContent))
                throw new ArgumentException("Script content cannot be null or empty", nameof(scriptContent));

            try
            {
                _databaseConnection.OpenConnection();
                var connection = _databaseConnection.GetConnection();

                using var command = connection.CreateCommand();
                command.CommandText = scriptContent;
                command.CommandTimeout = 300; // 5 minutes timeout
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to execute SQL script: {ex.Message}", ex);
            }
        }

        public async Task ExecuteScriptAsync(string scriptPath)
        {
            if (!File.Exists(scriptPath))
                throw new FileNotFoundException($"SQL script not found: {scriptPath}");

            var scriptContent = await File.ReadAllTextAsync(scriptPath);
            await ExecuteScriptContentAsync(scriptContent);
        }

        public async Task ExecuteScriptContentAsync(string scriptContent)
        {
            await Task.Run(() => ExecuteScriptContent(scriptContent));
        }
    }
}
