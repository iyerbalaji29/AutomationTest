namespace DataSetup.Interfaces
{
    /// <summary>
    /// Interface for SQL script execution following Single Responsibility Principle
    /// </summary>
    public interface ISqlScriptExecutor
    {
        void ExecuteScript(string scriptPath);
        void ExecuteScriptContent(string scriptContent);
        Task ExecuteScriptAsync(string scriptPath);
        Task ExecuteScriptContentAsync(string scriptContent);
    }
}
