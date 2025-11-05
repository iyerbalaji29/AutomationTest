# RunSettings Configuration Guide

This directory contains `.runsettings` files for the test automation framework. These files replace the previous `appsettings.json` configuration approach.

## Available RunSettings Files

### Environment-Based
- **Dev.runsettings** - Development environment configuration
- **QA.runsettings** - QA/Testing environment configuration
- **Prod.runsettings** - Production environment configuration

### Browser-Based
- **Chrome.runsettings** - Run entire test suite in Chrome browser
- **Firefox.runsettings** - Run entire test suite in Firefox browser
- **Edge.runsettings** - Run entire test suite in Edge browser

## Browser Configuration

**IMPORTANT**: The browser type is configured in the RunSettings file, NOT in individual tests.

- All tests use the `Browser` parameter from the selected RunSettings file
- To run tests in a different browser, simply select a different RunSettings file
- No browser-specific test categories or attributes are needed

## Configuration Parameters

Each `.runsettings` file contains the following parameters:

### Test Settings
- `BaseUrl` - The base URL for the application under test
- `Browser` - Browser to use (Chrome, Firefox, Edge)
- `ImplicitWaitSeconds` - Implicit wait timeout
- `ExplicitWaitSeconds` - Explicit wait timeout
- `PageLoadTimeoutSeconds` - Page load timeout
- `HeadlessMode` - Run tests in headless mode (true/false)
- `ScreenshotOnFailure` - Take screenshots on test failure (true/false)
- `ScreenshotPath` - Path to save screenshots

### Database Settings
- `ConnectionString` - Database connection string for test data setup

### Logging Settings
- `LogLevel` - Logging level (Debug, Information, Warning, Error)
- `LogPath` - Path to save log files

### Environment
- `Environment` - Current environment name (Dev, QA, Prod)

## Usage

### Visual Studio

1. Open Test Explorer
2. Click on the settings icon (gear)
3. Select "Configure Run Settings"
4. Choose "Select Solution Wide runsettings File"
5. Navigate to the appropriate `.runsettings` file (e.g., `RunSettings/Dev.runsettings`)

### Visual Studio Code

1. Open settings (Ctrl + ,)
2. Search for "dotnet.unitTests.runSettingsPath"
3. Set the path to your desired `.runsettings` file

### Command Line (dotnet test)

```bash
# Run tests with Dev settings
dotnet test --settings RunSettings/Dev.runsettings

# Run tests with QA settings
dotnet test --settings RunSettings/QA.runsettings

# Run tests with Prod settings
dotnet test --settings RunSettings/Prod.runsettings
```

### Command Line (vstest.console.exe)

```bash
vstest.console.exe UITests.dll /Settings:RunSettings/Dev.runsettings
```

## Customization

To create a new environment:

1. Copy an existing `.runsettings` file
2. Rename it (e.g., `Staging.runsettings`)
3. Update the parameter values for the new environment
4. Save and use it in your test runs

## Migration from appsettings.json

The project has been migrated from `appsettings.json` to `.runsettings` files. The main benefits are:

- Environment-specific configurations without code changes
- Better integration with Visual Studio Test Explorer
- Standard approach for .NET test configuration
- Easier CI/CD pipeline integration

The old `appsettings.json` files are no longer used by the framework and can be removed if desired.

## Example: Running Tests

### Run Tests by Environment
```bash
# Run all tests in Dev environment (uses Chrome by default)
dotnet test --settings RunSettings/Dev.runsettings

# Run all tests in QA environment
dotnet test --settings RunSettings/QA.runsettings

# Run all tests in Prod environment (headless mode)
dotnet test --settings RunSettings/Prod.runsettings
```

### Run Tests by Browser
```bash
# Run entire test suite in Chrome
dotnet test --settings RunSettings/Chrome.runsettings

# Run entire test suite in Firefox
dotnet test --settings RunSettings/Firefox.runsettings

# Run entire test suite in Edge
dotnet test --settings RunSettings/Edge.runsettings
```

### Run Specific Test Categories
```bash
# Run smoke tests in Chrome
dotnet test --settings RunSettings/Chrome.runsettings --filter "Category=Smoke"

# Run login tests in Firefox
dotnet test --settings RunSettings/Firefox.runsettings --filter "Category=Login"

# Run specific test by name in Edge
dotnet test --settings RunSettings/Edge.runsettings --filter "FullyQualifiedName~Login"
```

### Cross-Browser Testing
To run the same tests across multiple browsers, run the tests multiple times with different RunSettings:

```bash
# Run all smoke tests in all 3 browsers
dotnet test --settings RunSettings/Chrome.runsettings --filter "Category=Smoke"
dotnet test --settings RunSettings/Firefox.runsettings --filter "Category=Smoke"
dotnet test --settings RunSettings/Edge.runsettings --filter "Category=Smoke"
```

Or use a script/CI pipeline to automate cross-browser testing.

## Troubleshooting

If tests are not picking up the RunSettings:

1. Ensure the `.runsettings` file is properly formatted XML
2. Verify the file path is correct
3. Check that `TestConfiguration.Instance.InitializeFromTestContext()` is called in test setup
4. Restart Visual Studio or VS Code after changing settings

## Additional Resources

- [Microsoft Docs: Configure test runs with .runsettings](https://docs.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file)
