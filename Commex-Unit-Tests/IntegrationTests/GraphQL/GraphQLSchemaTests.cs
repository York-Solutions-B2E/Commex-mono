using NUnit.Framework;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Snapshooter.NUnit;
using System.Net.Http;
using System.Threading.Tasks;

namespace Commex_Unit_Tests.IntegrationTests.GraphQL
{
    [TestFixture]
    public class GraphQLSchemaTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        [Test]
        public async Task GraphQL_SchemaIntrospection_ContainsExpectedTypes()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail("Test not implemented");
        }

        [Test]
        public async Task GraphQL_Schema_ContainsQueryType()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail("Test not implemented");
        }

        [Test]
        public async Task GraphQL_Schema_ContainsMutationType()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail("Test not implemented");
        }

        [Test]
        public async Task GraphQL_Schema_ContainsSubscriptionType()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail("Test not implemented");
        }

        [Test]
        public async Task GraphQL_Schema_MatchesSnapshot()
        {
            // Arrange

            // Act

            // Assert - will use Snapshooter
            Assert.Fail("Test not implemented");
        }
    }
}