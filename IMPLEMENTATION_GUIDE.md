# GraphQL Implementation Guide for Commex

## Overview
This guide provides step-by-step instructions for implementing GraphQL functionality with TDD in the Commex application, based on the practice project requirements.

## Prerequisites
- Existing Commex backend and frontend running
- Docker infrastructure (SQL Server, RabbitMQ, Redis) operational
- Okta authentication configured and working
- Basic understanding of GraphQL concepts

## Phase 1: GraphQL Setup & TDD Test Harness

### Step 1: Install Required NuGet Packages
```bash
cd TSG-Commex-BE

# Core GraphQL packages
dotnet add package HotChocolate.AspNetCore
dotnet add package HotChocolate.Data
dotnet add package HotChocolate.Data.EntityFramework
dotnet add package HotChocolate.Subscriptions
dotnet add package HotChocolate.AspNetCore.Authorization

# For testing
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package FluentAssertions
dotnet add package Snapshooter.Xunit
```

### Step 2: Create Integration Test Project
```bash
# From solution root
dotnet new xunit -n TSG-Commex-BE.IntegrationTests
dotnet sln add TSG-Commex-BE.IntegrationTests

cd TSG-Commex-BE.IntegrationTests
dotnet add reference ../TSG-Commex-BE/TSG-Commex-BE.csproj
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package HotChocolate.AspNetCore.TestKit
dotnet add package FluentAssertions
```

### Step 3: Write Failing Tests First (TDD)
Create these test files BEFORE implementation:

1. **GraphQLAuthTests.cs** - Test authentication requirements
2. **GraphQLHealthTests.cs** - Test basic health query
3. **GraphQLSchemaTests.cs** - Test schema structure

Example test structure:
```csharp
// Test 1: Unauthorized access should return 401/403
[Fact]
public async Task GraphQL_UnauthorizedRequest_ReturnsUnauthorized()

// Test 2: Authorized health query should succeed
[Fact]
public async Task GraphQL_AuthorizedHealthQuery_ReturnsOk()

// Test 3: Schema should include Query, Mutation, Subscription
[Fact]
public async Task GraphQL_SchemaIntrospection_ContainsExpectedTypes()
```

### Step 4: Configure GraphQL in Program.cs
After tests are failing, implement:

1. Add GraphQL services to DI container
2. Configure authentication middleware order
3. Map GraphQL endpoint with auth
4. Conditionally enable Banana Cake Pop IDE

Key implementation points:
```csharp
// In Program.cs - Service Configuration
builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType(d => d.Name("Query"))
    .AddMutationType(d => d.Name("Mutation"))
    .AddSubscriptionType(d => d.Name("Subscription"))
    .AddProjections()
    .AddFiltering()
    .AddSorting();

// Middleware Pipeline (ORDER MATTERS!)
app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQL("/graphql");

if (app.Environment.IsDevelopment())
{
    app.MapBananaCakePop("/graphql/ui");
}
```

### Step 5: Create Minimal Query Type
```csharp
// Create GraphQL/Queries/HealthQuery.cs
public class HealthQuery
{
    [Authorize]
    public string Health() => "OK";
}
```

### Step 6: Configure Test Authentication
Create test JWT token generator or use test auth handler for integration tests.

### Step 7: Run Tests & Verify Green
```bash
dotnet test TSG-Commex-BE.IntegrationTests
```

## Phase 2: Communications List with Server Pagination

### Step 1: Write Failing Pagination Tests
Create tests BEFORE implementation:

1. **PaginationTests.cs** - Test cursor-based pagination
2. **FilteringTests.cs** - Test filtering by type and status
3. **SchemaSnapshotTests.cs** - Test result shape

Example tests:
```csharp
// Test 1: First page returns correct items with hasNextPage
[Fact]
public async Task Communications_FirstPage_ReturnsItemsWithPageInfo()

// Test 2: Using cursor returns next page
[Fact]
public async Task Communications_UsingCursor_ReturnsNextPage()

// Test 3: Result shape matches expected schema
[Fact]
public async Task Communications_ResultShape_MatchesSnapshot()
```

### Step 2: Extend Query Type with Communications
```csharp
// GraphQL/Queries/CommunicationQuery.cs
public class CommunicationQuery
{
    [Authorize]
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Communication> GetCommunications(
        [Service] ApplicationDbContext context)
    {
        return context.Communications
            .Include(c => c.StatusHistory)
            .OrderBy(c => c.LastUpdatedUtc)
            .ThenBy(c => c.Id);
    }
}
```

### Step 3: Configure Relay-Style Pagination
Update GraphQL configuration:
```csharp
builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>()
    .AddTypeExtension<CommunicationQuery>()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .SetPagingOptions(new PagingOptions
    {
        MaxPageSize = 50,
        DefaultPageSize = 10,
        IncludeTotalCount = true
    });
```

### Step 4: Create GraphQL Types
```csharp
// GraphQL/Types/CommunicationType.cs
public class CommunicationType : ObjectType<Communication>
{
    protected override void Configure(IObjectTypeDescriptor<Communication> descriptor)
    {
        descriptor
            .Field(c => c.Id)
            .Type<NonNullType<IdType>>();
        
        descriptor
            .Field(c => c.StatusHistory)
            .UseFiltering()
            .UseSorting();
    }
}
```

### Step 5: Implement Stable Cursor Strategy
Create cursor based on composite key (LastUpdatedUtc + Id) for consistency.

### Step 6: Test Query Examples
```graphql
# Test query for first page
query {
  communications(first: 2) {
    edges {
      cursor
      node {
        id
        trackingNumber
        recipientName
        currentStatus
        lastUpdatedUtc
      }
    }
    pageInfo {
      hasNextPage
      hasPreviousPage
      startCursor
      endCursor
    }
    totalCount
  }
}

# Test query with cursor
query {
  communications(first: 2, after: "cursor_value_here") {
    edges {
      cursor
      node {
        id
        trackingNumber
      }
    }
    pageInfo {
      hasNextPage
      endCursor
    }
  }
}
```

### Step 7: Configure Nitro/Swagger for Okta Token
Update Swagger configuration to include OAuth2 flow for testing GraphQL with real tokens.

## Testing Checklist

### Phase 1 Tests
- [ ] Unauthorized GraphQL request returns 401/403
- [ ] Authorized health query returns 200 OK
- [ ] Schema introspection includes Query, Mutation, Subscription types
- [ ] Banana Cake Pop only available in Development
- [ ] Integration tests use WebApplicationFactory

### Phase 2 Tests
- [ ] First page returns exactly requested items
- [ ] hasNextPage correctly indicates more data
- [ ] Cursor from endCursor retrieves next page
- [ ] Filters work (by type, by status)
- [ ] Combined filters work correctly
- [ ] Result shape matches expected schema
- [ ] Maximum page size is enforced (50 items)

## Validation Steps

1. **Run all tests**
   ```bash
   dotnet test
   ```

2. **Test GraphQL endpoint manually**
   - Navigate to https://localhost:7019/graphql/ui (dev only)
   - Authenticate with Okta
   - Run test queries

3. **Verify authentication**
   - Test unauthorized access gets rejected
   - Test authorized access succeeds

4. **Verify pagination**
   - Query first page
   - Use cursor to get next page
   - Apply filters and verify results

## Common Issues & Solutions

### Issue: Tests fail with auth errors
**Solution**: Ensure test JWT matches Okta configuration (audience, issuer)

### Issue: Pagination cursor not stable
**Solution**: Use composite key (timestamp + id) for cursor generation

### Issue: Banana Cake Pop not loading
**Solution**: Check it's only mapped in Development environment

### Issue: Filters not working
**Solution**: Ensure UseFiltering() is added to field configuration

## Next Steps

After completing both stories:
1. Add more query fields for other entities
2. Implement mutations for CRUD operations
3. Add real-time subscriptions for status updates
4. Create DataLoader for N+1 query optimization
5. Add field-level authorization
6. Implement custom scalar types if needed

## Resources

- [HotChocolate Documentation](https://chillicream.com/docs/hotchocolate)
- [GraphQL Relay Specification](https://relay.dev/graphql/connections.htm)
- [TDD in .NET](https://docs.microsoft.com/en-us/dotnet/core/testing/)
- [WebApplicationFactory Testing](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests)