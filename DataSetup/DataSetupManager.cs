using DataSetup.Implementations;
using DataSetup.Interfaces;

namespace DataSetup
{
    /// <summary>
    /// Facade for data setup operations
    /// Provides simplified interface for test data management
    /// </summary>
    public class DataSetupManager
    {
        private readonly ISqlScriptExecutor _scriptExecutor;
        private readonly string _sqlScriptsBasePath;

        public DataSetupManager(string connectionString, string sqlScriptsBasePath = "SqlScripts")
        {
            var dbConnection = new SqlDatabaseConnection(connectionString);
            _scriptExecutor = new SqlScriptExecutor(dbConnection);
            _sqlScriptsBasePath = sqlScriptsBasePath;
        }

        public DataSetupManager(ISqlScriptExecutor scriptExecutor, string sqlScriptsBasePath = "SqlScripts")
        {
            _scriptExecutor = scriptExecutor ?? throw new ArgumentNullException(nameof(scriptExecutor));
            _sqlScriptsBasePath = sqlScriptsBasePath;
        }

        /// <summary>
        /// Executes a SQL script from the SqlScripts folder
        /// </summary>
        public void ExecuteSetupScript(string scriptFileName)
        {
            var scriptPath = Path.Combine(_sqlScriptsBasePath, scriptFileName);
            _scriptExecutor.ExecuteScript(scriptPath);
        }

        /// <summary>
        /// Executes multiple SQL scripts in sequence
        /// </summary>
        public void ExecuteSetupScripts(params string[] scriptFileNames)
        {
            foreach (var scriptFileName in scriptFileNames)
            {
                ExecuteSetupScript(scriptFileName);
            }
        }

        /// <summary>
        /// Executes SQL script content directly
        /// </summary>
        public void ExecuteSetupScriptContent(string scriptContent)
        {
            _scriptExecutor.ExecuteScriptContent(scriptContent);
        }

        /// <summary>
        /// Executes a SQL script asynchronously
        /// </summary>
        public async Task ExecuteSetupScriptAsync(string scriptFileName)
        {
            var scriptPath = Path.Combine(_sqlScriptsBasePath, scriptFileName);
            await _scriptExecutor.ExecuteScriptAsync(scriptPath);
        }
    }
}
