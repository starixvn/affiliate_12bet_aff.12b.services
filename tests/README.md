# Aff12B.Services Test Infrastructure

This directory contains the comprehensive unit testing infrastructure for the Aff12B.Services solution, implementing **Phase 1.1** of the Unit Testing Plan.

## 📁 Project Structure

```
tests/
├── Aff12B.Common.Tests/                    # Shared test infrastructure
│   ├── Builders/                           # Test data builders
│   │   ├── CustomerModelBuilder.cs         # Builder for CustomerModel
│   │   └── AffCustomerModelBuilder.cs      # Builder for AffCustomerModel
│   ├── Fixtures/                           # Shared test fixtures
│   │   └── CommonTestFixture.cs            # Common test setup and services
│   ├── Mocks/                              # Common mock setups
│   │   └── MockRepositoryHelper.cs         # Repository mocking utilities
│   ├── TestUtilities.cs                   # Core test helper methods
│   └── TestInfrastructureTests.cs         # Infrastructure validation tests
├── Aff12B.Common.DataDomain.Tests/        # Data models & DTOs tests (25+ classes)
├── Aff12B.Common.Infrastructure.Tests/    # Infrastructure helpers & middleware tests (4 classes)
├── Aff12B.Ext.API.Core.Tests/            # API core business logic tests (4 classes)
├── Aff12B.Ext.API.Infrastructure.Tests/  # API infrastructure tests (25+ classes)
├── Aff12B.Ext.API.Site.Tests/            # API controllers & integration tests (8+ classes)
├── Aff12B.CronJobs.Core.Tests/           # CronJob interfaces tests (15 interfaces)
├── Aff12B.CronJobs.Infrastructure.Tests/ # CronJob implementations tests (50+ classes)
└── TestResults/                           # Test execution and coverage reports
    ├── Coverage/                          # Coverage reports
    └── Reports/                           # Test execution reports
```

## 🛠️ Technology Stack

### Testing Framework
- **xUnit 2.6.1** - Primary testing framework
- **Microsoft.NET.Test.Sdk 17.8.0** - .NET test SDK
- **xunit.runner.visualstudio 2.5.3** - Visual Studio test runner

### Mocking & Assertions
- **Moq 4.20.69** - Mocking framework
- **FluentAssertions 6.12.0** - Fluent assertion library
- **AutoFixture 4.18.0** - Test data generation
- **AutoFixture.Xunit2 4.18.0** - xUnit integration

### Integration Testing
- **Microsoft.AspNetCore.Mvc.Testing 8.0.0** - ASP.NET Core integration testing
- **Microsoft.Extensions.DependencyInjection.Abstractions 8.0.2** - Dependency injection

### Coverage & Reporting
- **coverlet.collector 6.0.0** - Code coverage collection
- **Coverage threshold: 80%** minimum line coverage
- **ReportGenerator** - HTML coverage reports

## 🔧 Key Features

### TestUtilities Class
```csharp
// Create configured AutoFixture
var fixture = TestUtilities.CreateFixture();

// Create mock logger
var mockLogger = TestUtilities.CreateMockLogger<MyService>();

// Create test configuration
var tokenConfig = TestUtilities.CreateTestTokenManagement();
var testUsers = TestUtilities.CreateTestUsers();

// Create test models
var customer = TestUtilities.CreateTestCustomerModel();
var affCustomer = TestUtilities.CreateTestAffCustomerModel();

// Verify logging
TestUtilities.VerifyLoggerCalled(mockLogger, LogLevel.Error, Times.Once);

// Create date ranges
var (from, to) = TestUtilities.CreateDateRange(daysBack: 7, daysForward: 1);
```

### Builder Pattern Support
```csharp
// CustomerModel builder
var customer = new CustomerModelBuilder()
    .WithCustId(12345)
    .WithUserName("testuser")
    .AsTestCustomer()
    .RegisteredDaysAgo(30)
    .Build();

// AffCustomerModel builder
var affCustomer = new AffCustomerModelBuilder()
    .WithCustId(12345)
    .WithClickId(98765)
    .FirstDepositDaysAgo(7)
    .WithoutFirstDeposit()
    .Build();
```

### Mock Repository Helper
```csharp
// Setup async method return
MockRepositoryHelper.SetupAsyncResult(
    mockRepo, 
    x => x.GetCustomerAsync(It.IsAny<int>()), 
    customer);

// Setup async list return
MockRepositoryHelper.SetupAsyncListResult(
    mockRepo, 
    x => x.GetCustomersAsync(), 
    customers);

// Verify method calls
MockRepositoryHelper.VerifyAsyncMethodCalledOnce(
    mockRepo, 
    x => x.GetCustomerAsync(It.IsAny<int>()));
```

### Common Test Fixture
```csharp
public class MyServiceTests : IClassFixture<CommonTestFixture>
{
    private readonly CommonTestFixture _fixture;

    public MyServiceTests(CommonTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void MyTest()
    {
        // Use shared services
        var logger = _fixture.CreateMockLogger<MyService>();
        var fixture = _fixture.Fixture;
        
        // Test implementation
    }
}
```

## 📊 Coverage Configuration

The solution is configured with **coverlet.runsettings** for:
- **80% minimum line coverage** threshold
- **Cobertura XML format** for CI/CD integration
- **Exclusions** for Program.cs, Startup.cs, Constants, and generated code
- **HTML reports** in `./tests/TestResults/Coverage/`

## 🚀 Usage Commands

### Build & Test
```bash
# Restore packages
dotnet restore

# Build solution
dotnet build --configuration Release

# Run all tests
dotnet test --configuration Release --verbosity normal

# Run tests with coverage
dotnet test --configuration Release --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

### Coverage Reports
```bash
# Generate HTML coverage report
reportgenerator \
  -reports:"./tests/TestResults/**/coverage.cobertura.xml" \
  -targetdir:"./tests/TestResults/Coverage" \
  -reporttypes:"Html;Badges;TextSummary"
```

## 📋 Implementation Status

### ✅ Completed (Phase 1 - Complete Infrastructure)
- [x] **Test project structure** - All 8 test projects created with subdirectories
- [x] **Shared test infrastructure** - TestUtilities, Builders, Fixtures, Mocks
- [x] **Coverage configuration** - coverlet.runsettings with 80% threshold
- [x] **Solution integration** - All projects added to solution
- [x] **Build verification** - All projects compile successfully
- [x] **Test framework validation** - Infrastructure tests pass
- [x] **Additional builders** - BonusModel, TpBetRecord, DepositInfo, TagModel builders
- [x] **API test utilities** - Authentication, request creation, response parsing
- [x] **Integration test base** - WebApplicationFactory with test configurations
- [x] **Domain test fixtures** - CronJobs domain test doubles and fixtures

### 🔄 Next Phases (To Be Implemented)
- [ ] **Phase 2**: High-Priority Core Components (Data Models, Parameters, Exceptions)
- [ ] **Phase 3**: Authentication & Security Layer
- [ ] **Phase 4**: API Controllers Testing
- [ ] **Phase 5**: Utility & Helper Classes
- [ ] **Phase 6**: Repository Layer Testing
- [ ] **Phase 7**: Service Layer Testing
- [ ] **Phase 8**: Middleware & Infrastructure
- [ ] **Phase 9**: Configuration & Criteria Classes
- [ ] **Phase 10**: Background Jobs & Base Classes
- [ ] **Phase 11**: Integration Testing

## 🎯 Coverage Targets

| Component | Classes | Target Coverage | Priority |
|-----------|---------|-----------------|----------|
| **Controllers** | 8 classes | **95%+** | Critical |
| **Services** | 4 classes | **90%+** | Critical |
| **Models/DTOs** | 25+ classes | **95%+** | High |
| **Repositories** | 5 classes | **90%+** | High |
| **Helpers/Utils** | 6 classes | **95%+** | High |
| **Middleware** | 3 classes | **85%+** | Medium |
| **Extensions** | 4 classes | **100%** | Medium |
| **Exceptions** | 2 classes | **100%** | Medium |
| **Overall Project** | **164 classes** | **>80%** | **Required** |

## 🔍 Quality Assurance

### Test Principles
- ✅ **Unit isolation** - Each test is independent
- ✅ **Comprehensive mocking** - External dependencies mocked
- ✅ **Fluent assertions** - Readable and maintainable assertions
- ✅ **Builder patterns** - Consistent test data creation
- ✅ **Shared infrastructure** - Reusable test utilities
- ✅ **Coverage monitoring** - Automated threshold enforcement

### CI/CD Integration
- ✅ **Automated testing** - All tests run on every commit
- ✅ **Coverage reporting** - Coverage reports generated automatically
- ✅ **Quality gates** - Build fails if coverage drops below 80%
- ✅ **Performance monitoring** - Test execution time tracking

---

**Phase 1.1 Complete** ✅  
**Ready for Phase 2 Implementation** 🚀

The foundation is now in place to achieve the **>80% code coverage** target across all 164 classes in the Aff12B.Services solution.
