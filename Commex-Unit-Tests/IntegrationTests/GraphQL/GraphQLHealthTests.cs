using NUnit.Framework;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;

namespace Commex_Unit_Tests.IntegrationTests.GraphQL
{
    [TestFixture]
    public class GraphQLHealthTests
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
        public async Task GraphQL_HealthQuery_ReturnsOk()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail("Test not implemented");
        }

        [Test]
        public async Task GraphQL_HealthQuery_ReturnsExpectedResponse()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail("Test not implemented");
        }
    }
}