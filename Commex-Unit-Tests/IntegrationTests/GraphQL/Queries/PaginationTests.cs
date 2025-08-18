using NUnit.Framework;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;

namespace Commex_Unit_Tests.IntegrationTests.GraphQL.Queries
{
    [TestFixture]
    public class PaginationTests
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
        public async Task Communications_FirstPage_ReturnsItemsWithPageInfo()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail("Test not implemented");
        }

        [Test]
        public async Task Communications_UsingCursor_ReturnsNextPage()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail("Test not implemented");
        }

        [Test]
        public async Task Communications_PageSize_RespectedCorrectly()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail("Test not implemented");
        }

        [Test]
        public async Task Communications_LastPage_HasNextPageFalse()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail("Test not implemented");
        }

        [Test]
        public async Task Communications_MaxPageSize_EnforcedCorrectly()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail("Test not implemented");
        }

        [Test]
        public async Task Communications_ResultShape_MatchesSnapshot()
        {
            // Arrange

            // Act

            // Assert - will use Snapshooter
            Assert.Fail("Test not implemented");
        }
    }
}