# API Testing Framework with Playwright and ExtentReports

This project is an automated API testing framework built with Playwright, xUnit, and ExtentReports. It provides comprehensive API testing capabilities with detailed reporting and screenshot capture functionality.

## Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or later
- [Node.js](https://nodejs.org/) (for Playwright)
- [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

## Project Structure

```
PlaywrightTests/
├── test/                    # Test classes
├── Utilities/              # Helper classes and utilities
│   ├── TBase.cs           # Base test class
│   ├── ExtentManager.cs   # ExtentReports manager
│   └── ApiClient.cs       # API client wrapper
├── Reports/               # Generated test reports
├── Screenshots/          # Captured screenshots
└── .env                  # Environment configuration
```

## Setup Instructions

1. **Clone the Repository**
   ```bash
   git clone <repository-url>
   cd <project-directory>
   ```

2. **Install Dependencies**
   ```bash
   # Install .NET dependencies
   dotnet restore

   # Install Playwright browsers
   pwsh bin/Debug/net6.0/playwright.ps1 install
   ```

3. **Environment Configuration**
   - Create a `.env` file in the `PlaywrightTests` directory
   - Add the following environment variables:
     ```
     BASE_URL=your_api_base_url
     AUTH_TOKEN=your_auth_token
     ```

4. **Build the Project**
   ```bash
   dotnet build
   ```

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~PostRequestTests"
```

### Run Specific Test Method
```bash
dotnet test --filter "FullyQualifiedName=PlaywrightTests.test.PostRequestTests.CreatePost_ShouldReturn201"
```

## Test Reports

After test execution, reports are generated in the `Reports` directory:
- HTML Report: `Reports/extent-report_[timestamp].html`
- JSON Report: `Reports/extent-report_[timestamp].json`

### Viewing Reports
1. Open the HTML report in any web browser
2. The report includes:
   - Test execution summary
   - Detailed test steps
   - Request/Response details
   - Screenshots (for failed tests)
   - Error messages and stack traces

## Screenshots

Screenshots are automatically captured for failed tests and stored in the `Screenshots` directory:
- Format: `[TestName]_[timestamp].png`
- Full-page screenshots are captured
- Screenshots are attached to the test reports

## Key Features

1. **API Testing**
   - HTTP methods support (GET, POST, PUT, DELETE)
   - Request/Response logging
   - Schema validation
   - Authentication handling

2. **Reporting**
   - Detailed HTML reports
   - JSON format reports
   - Screenshot capture
   - Request/Response logging
   - Error tracking

3. **Test Organization**
   - Page Object Model
   - Base test class
   - Utility classes
   - Environment configuration

## Troubleshooting

1. **Playwright Installation Issues**
   ```bash
   # Reinstall Playwright browsers
   pwsh bin/Debug/net6.0/playwright.ps1 install --force
   ```

2. **Report Generation Issues**
   - Ensure the `Reports` directory exists
   - Check write permissions
   - Verify test execution completed

3. **Screenshot Issues**
   - Ensure the `Screenshots` directory exists
   - Check browser initialization
   - Verify test failure handling

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

[Your License Here]

## Contact

[Your Contact Information] 