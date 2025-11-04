# DataSetup Project

This project manages test data setup and teardown using ADO.NET for SQL Server database operations.

## Purpose

The DataSetup project provides a clean, maintainable way to:
- Execute SQL scripts for test data creation
- Manage database connections
- Clean up test data after execution
- Support data-driven testing scenarios

## Architecture

The project follows SOLID principles with clear separation of concerns:

```
DataSetup/
├── Interfaces/              # Contracts (DIP)
│   ├── IDatabaseConnection
│   └── ISqlScriptExecutor
├── Implementations/         # Concrete implementations
│   ├── SqlDatabaseConnection
│   └── SqlScriptExecutor
├── SqlScripts/             # SQL script storage
│   ├── SampleData/         # Test data creation
│   └── Cleanup/            # Test data cleanup
└── DataSetupManager.cs     # Facade pattern
```

## Key Components

### IDatabaseConnection
Interface for database connection management.

```csharp
public interface IDatabaseConnection
{
    IDbConnection GetConnection();
    void OpenConnection();
    void CloseConnection();
}
```

### SqlDatabaseConnection
SQL Server implementation of database connections.

**Responsibilities:**
- Create and manage SQL Server connections
- Handle connection lifecycle
- Implement IDisposable for resource cleanup

**Usage:**
```csharp
using var dbConnection = new SqlDatabaseConnection(connectionString);
dbConnection.OpenConnection();
// Perform operations
dbConnection.CloseConnection();
```

### ISqlScriptExecutor
Interface for SQL script execution.

```csharp
public interface ISqlScriptExecutor
{
    void ExecuteScript(string scriptPath);
    void ExecuteScriptContent(string scriptContent);
    Task ExecuteScriptAsync(string scriptPath);
    Task ExecuteScriptContentAsync(string scriptContent);
}
```

### SqlScriptExecutor
Executes SQL scripts against the database.

**Features:**
- Synchronous and asynchronous execution
- File-based and content-based execution
- 5-minute command timeout
- Comprehensive error handling

**Usage:**
```csharp
var executor = new SqlScriptExecutor(databaseConnection);
executor.ExecuteScript("SqlScripts/SampleData/CreateTestUsers.sql");
```

### DataSetupManager
Facade providing simplified access to data setup operations.

**Usage:**
```csharp
var dataSetupManager = new DataSetupManager(connectionString);

// Execute single script
dataSetupManager.ExecuteSetupScript("SampleData/CreateTestUsers.sql");

// Execute multiple scripts
dataSetupManager.ExecuteSetupScripts(
    "SampleData/CreateTestUsers.sql",
    "SampleData/CreateTestProducts.sql"
);

// Execute cleanup
dataSetupManager.ExecuteSetupScript("Cleanup/CleanupTestUsers.sql");
```

## SQL Scripts Organization

### Directory Structure

```
SqlScripts/
├── SampleData/              # Data creation scripts
│   ├── CreateTestUsers.sql
│   ├── CreateTestProducts.sql
│   └── CreateTestOrders.sql
└── Cleanup/                 # Data cleanup scripts
    ├── CleanupTestUsers.sql
    ├── CleanupTestProducts.sql
    └── CleanupTestOrders.sql
```

### Script Naming Conventions

- **Creation scripts**: `Create{EntityName}.sql`
- **Cleanup scripts**: `Cleanup{EntityName}.sql`
- Use descriptive names that indicate the data being created/cleaned

### Script Best Practices

1. **Idempotent Scripts**: Scripts should be safe to run multiple times
   ```sql
   IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TestUsers')
   BEGIN
       CREATE TABLE TestUsers (...)
   END
   ```

2. **Conditional Inserts**: Check for existing data before inserting
   ```sql
   IF NOT EXISTS (SELECT * FROM TestUsers WHERE Email = 'test@example.com')
   BEGIN
       INSERT INTO TestUsers (...) VALUES (...)
   END
   ```

3. **Cleanup Safety**: Use WHERE clauses to avoid deleting production data
   ```sql
   DELETE FROM TestUsers WHERE Email LIKE '%@example.com';
   ```

## Adding New SQL Scripts

### Step 1: Create the SQL File

Create a new `.sql` file in the appropriate folder:
- `SqlScripts/SampleData/` for data creation
- `SqlScripts/Cleanup/` for data cleanup

### Step 2: Write the Script

```sql
-- CreateTestCustomers.sql
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TestCustomers')
BEGIN
    CREATE TABLE TestCustomers (
        CustomerId INT PRIMARY KEY IDENTITY(1,1),
        CustomerName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL,
        CreatedDate DATETIME DEFAULT GETDATE()
    );
END

IF NOT EXISTS (SELECT * FROM TestCustomers WHERE Email = 'customer1@test.com')
BEGIN
    INSERT INTO TestCustomers (CustomerName, Email)
    VALUES ('Test Customer 1', 'customer1@test.com');
END
```

### Step 3: Create Cleanup Script

```sql
-- CleanupTestCustomers.sql
DELETE FROM TestCustomers WHERE Email LIKE '%@test.com';
-- DROP TABLE IF EXISTS TestCustomers;
```

### Step 4: Use in Tests

```csharp
[SetUp]
public void SetupTestData()
{
    var dataManager = new DataSetupManager(connectionString);
    dataManager.ExecuteSetupScript("SampleData/CreateTestCustomers.sql");
}

[TearDown]
public void CleanupTestData()
{
    var dataManager = new DataSetupManager(connectionString);
    dataManager.ExecuteSetupScript("Cleanup/CleanupTestCustomers.sql");
}
```

## Integration with Tests

### Using with NUnit

```csharp
[TestFixture]
public class DataDrivenTests
{
    private DataSetupManager _dataManager;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var config = TestConfiguration.Instance;
        _dataManager = new DataSetupManager(config.ConnectionString);
    }

    [SetUp]
    public void Setup()
    {
        _dataManager.ExecuteSetupScript("SampleData/CreateTestUsers.sql");
    }

    [TearDown]
    public void Teardown()
    {
        _dataManager.ExecuteSetupScript("Cleanup/CleanupTestUsers.sql");
    }

    [Test]
    public void TestWithData()
    {
        // Test implementation using seeded data
    }
}
```

### Using with ReqNRoll

```csharp
[Binding]
public class DataSetupHooks
{
    private DataSetupManager _dataManager;

    [BeforeScenario("@database")]
    public void SetupDatabase()
    {
        var config = TestConfiguration.Instance;
        _dataManager = new DataSetupManager(config.ConnectionString);
        _dataManager.ExecuteSetupScript("SampleData/CreateTestUsers.sql");
    }

    [AfterScenario("@database")]
    public void CleanupDatabase()
    {
        _dataManager.ExecuteSetupScript("Cleanup/CleanupTestUsers.sql");
    }
}
```

## Configuration

Update the connection string in `appsettings.json`:

```json
{
  "DatabaseSettings": {
    "ConnectionString": "Server=localhost;Database=TestDB;Integrated Security=true;TrustServerCertificate=True;"
  }
}
```

### Connection String Formats

**Windows Authentication:**
```
Server=localhost;Database=TestDB;Integrated Security=true;TrustServerCertificate=True;
```

**SQL Server Authentication:**
```
Server=localhost;Database=TestDB;User Id=testuser;Password=testpass;TrustServerCertificate=True;
```

**Azure SQL Database:**
```
Server=tcp:myserver.database.windows.net,1433;Database=TestDB;User Id=testuser;Password=testpass;Encrypt=True;
```

## Advanced Usage

### Async Script Execution

```csharp
await _dataManager.ExecuteSetupScriptAsync("SampleData/LargeDataSet.sql");
```

### Custom Script Executor

```csharp
var dbConnection = new SqlDatabaseConnection(connectionString);
var scriptExecutor = new SqlScriptExecutor(dbConnection);

// Execute from file
scriptExecutor.ExecuteScript("path/to/script.sql");

// Execute inline SQL
scriptExecutor.ExecuteScriptContent(@"
    INSERT INTO TestUsers (UserName, Email)
    VALUES ('Dynamic User', 'dynamic@test.com')
");
```

### Parameterized Scripts

For parameterized queries, execute inline SQL with parameters:

```csharp
var sql = $@"
    INSERT INTO TestUsers (UserName, Email)
    VALUES ('{userName}', '{email}')
";
_dataManager.ExecuteSetupScriptContent(sql);
```

**Note**: For production use, implement proper parameterization to prevent SQL injection.

## Error Handling

The framework provides comprehensive error handling:

```csharp
try
{
    _dataManager.ExecuteSetupScript("SampleData/CreateTestUsers.sql");
}
catch (FileNotFoundException ex)
{
    // Script file not found
    Console.WriteLine($"Script not found: {ex.Message}");
}
catch (InvalidOperationException ex)
{
    // SQL execution failed
    Console.WriteLine($"Execution failed: {ex.Message}");
}
```

## Testing Best Practices

1. **Isolate Test Data**: Use unique identifiers (e.g., email domains) to isolate test data
2. **Clean Up**: Always clean up test data in [TearDown] or [AfterScenario]
3. **Transaction Scope**: Consider using transactions for rollback capability
4. **Parallel Execution**: Ensure test data doesn't conflict when running tests in parallel
5. **Data Factories**: Create reusable data factory methods for common scenarios

## Maintenance

### Regular Tasks

1. **Review Scripts**: Periodically review and update SQL scripts
2. **Optimize Performance**: Monitor script execution times
3. **Database Cleanup**: Ensure cleanup scripts are comprehensive
4. **Version Control**: Keep scripts in version control
5. **Documentation**: Document complex data setups

### Extending Functionality

To support other databases (e.g., MySQL, PostgreSQL):

1. Create new implementation of `IDatabaseConnection`
2. Create new implementation of `ISqlScriptExecutor` if needed
3. Update `DataSetupManager` to support multiple database types

## Dependencies

- **System.Data.SqlClient** (4.8.6): SQL Server connectivity
- **Microsoft.Extensions.Configuration** (8.0.0): Configuration management

## Troubleshooting

**Issue: Connection Timeout**
- Increase connection timeout in connection string
- Check database server accessibility

**Issue: Script Execution Fails**
- Verify SQL syntax
- Check user permissions
- Review script for database-specific features

**Issue: Cleanup Not Working**
- Verify WHERE clauses in cleanup scripts
- Check for foreign key constraints
- Review transaction scope

---

**For more information**, refer to the main [README.md](../README.md)
