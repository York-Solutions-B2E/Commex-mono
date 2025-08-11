using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using TSG_Commex_BE.Data;
using TSG_Commex_BE.Models.Domain;
using TSG_Commex_BE.Models.Enums;
using TSG_Commex_BE.Repositories.Implementations;
using TSG_Commex_BE.Repositories.Interfaces;

namespace Commex_Unit_Tests.Repositories
{
    [TestFixture]
    public class CommunicationRepositoryTests
    {
        private ApplicationDbContext _context;
        private ICommunicationRepository _repository;

        [SetUp]
        public void Setup()
        {
            // Use in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            
            _context = new ApplicationDbContext(options);
            _repository = new CommunicationRepository(_context);
            
            // Seed test data
            SeedTestData();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private void SeedTestData()
        {
            var user = new User 
            { 
                Id = 1, 
                Email = "test@example.com", 
                FirstName = "Test", 
                LastName = "User" 
            };

            var member = new Member 
            { 
                Id = 1, 
                MemberId = "M12345",
                FirstName = "John", 
                LastName = "Doe",
                Email = "john@example.com",
                IsActive = true
            };

            var status = new GlobalStatus 
            { 
                Id = 1, 
                StatusCode = "ReadyForRelease",
                DisplayName = "Ready for Release",
                Description = "Ready to be released",
                Phase = StatusPhase.Creation,
                IsActive = true
            };

            var communicationType = new CommunicationType 
            { 
                Id = 1, 
                TypeCode = "EOB",
                DisplayName = "Explanation of Benefits",
                Description = "EOB Documents",
                IsActive = true
            };

            _context.Users.Add(user);
            _context.Members.Add(member);
            _context.GlobalStatuses.Add(status);
            _context.CommunicationTypes.Add(communicationType);
            _context.SaveChanges();
        }

        #region GetAllAsync Tests

        [Test]
        public async Task GetAllAsync_ReturnsOnlyActiveCommunications()
        {
            // Arrange
            var activeCommunication = new Communication
            {
                Id = 1,
                Title = "Active Communication",
                MemberId = 1,
                CommunicationTypeId = 1,
                CurrentStatusId = 1,
                CreatedByUserId = 1,
                IsActive = true,
                CreatedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow
            };

            var inactiveCommunication = new Communication
            {
                Id = 2,
                Title = "Inactive Communication",
                MemberId = 1,
                CommunicationTypeId = 1,
                CurrentStatusId = 1,
                CreatedByUserId = 1,
                IsActive = false,
                CreatedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow
            };

            _context.Communications.AddRange(activeCommunication, inactiveCommunication);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Active Communication"));
        }

        [Test]
        public async Task GetAllAsync_IncludesNavigationProperties()
        {
            // Arrange
            var communication = new Communication
            {
                Id = 1,
                Title = "Test Communication",
                MemberId = 1,
                CommunicationTypeId = 1,
                CurrentStatusId = 1,
                CreatedByUserId = 1,
                IsActive = true,
                CreatedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow
            };

            _context.Communications.Add(communication);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();
            var retrieved = result.First();

            // Assert
            Assert.That(retrieved.CommunicationType, Is.Not.Null);
            Assert.That(retrieved.CurrentStatus, Is.Not.Null);
            Assert.That(retrieved.CreatedByUser, Is.Not.Null);
        }

        #endregion

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsCommunication()
        {
            // Arrange
            var communication = new Communication
            {
                Id = 1,
                Title = "Test Communication",
                MemberId = 1,
                CommunicationTypeId = 1,
                CurrentStatusId = 1,
                CreatedByUserId = 1,
                IsActive = true,
                CreatedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow
            };

            _context.Communications.Add(communication);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Title, Is.EqualTo("Test Communication"));
        }

        [Test]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetByIdAsync_InactiveCommunication_ReturnsNull()
        {
            // Arrange
            var communication = new Communication
            {
                Id = 1,
                Title = "Inactive Communication",
                MemberId = 1,
                CommunicationTypeId = 1,
                CurrentStatusId = 1,
                CreatedByUserId = 1,
                IsActive = false,
                CreatedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow
            };

            _context.Communications.Add(communication);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region CreateAsync Tests

        [Test]
        public async Task CreateAsync_ValidCommunication_SavesAndReturnsEntity()
        {
            // Arrange
            var communication = new Communication
            {
                Title = "New Communication",
                MemberId = 1,
                CommunicationTypeId = 1,
                CurrentStatusId = 1,
                CreatedByUserId = 1,
                IsActive = true
            };

            // Act
            var result = await _repository.CreateAsync(communication);

            // Assert
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.CreatedUtc, Is.Not.EqualTo(DateTime.MinValue));
            Assert.That(result.LastUpdatedUtc, Is.Not.EqualTo(DateTime.MinValue));

            // Verify it was actually saved
            var saved = await _context.Communications.FindAsync(result.Id);
            Assert.That(saved, Is.Not.Null);
            Assert.That(saved.Title, Is.EqualTo("New Communication"));
        }

        #endregion

        #region UpdateAsync Tests

        [Test]
        public async Task UpdateAsync_ExistingEntity_UpdatesDatabase()
        {
            // Arrange
            var communication = new Communication
            {
                Id = 1,
                Title = "Original Title",
                MemberId = 1,
                CommunicationTypeId = 1,
                CurrentStatusId = 1,
                CreatedByUserId = 1,
                IsActive = true,
                CreatedUtc = DateTime.UtcNow.AddDays(-1),
                LastUpdatedUtc = DateTime.UtcNow.AddDays(-1)
            };

            _context.Communications.Add(communication);
            await _context.SaveChangesAsync();
            _context.Entry(communication).State = EntityState.Detached;

            // Act
            communication.Title = "Updated Title";
            await _repository.UpdateAsync(communication);

            // Assert
            var updated = await _context.Communications.FindAsync(1);
            Assert.That(updated.Title, Is.EqualTo("Updated Title"));
            Assert.That(updated.LastUpdatedUtc, Is.GreaterThan(communication.CreatedUtc));
        }

        #endregion

        #region DeleteAsync Tests

        [Test]
        public async Task DeleteAsync_ExistingId_SoftDeletes()
        {
            // Arrange
            var communication = new Communication
            {
                Id = 1,
                Title = "To Delete",
                MemberId = 1,
                CommunicationTypeId = 1,
                CurrentStatusId = 1,
                CreatedByUserId = 1,
                IsActive = true,
                CreatedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow
            };

            _context.Communications.Add(communication);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(1);

            // Assert
            var deleted = await _context.Communications.FindAsync(1);
            Assert.That(deleted, Is.Not.Null);
            Assert.That(deleted.IsActive, Is.False);
        }

        [Test]
        public async Task DeleteAsync_NonExistingId_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await _repository.DeleteAsync(999));
        }

        #endregion

        #region GetByStatusAsync Tests

        [Test]
        public async Task GetByStatusAsync_ReturnsFilteredResults()
        {
            // Arrange
            var comm1 = new Communication
            {
                Id = 1,
                Title = "Status 1",
                MemberId = 1,
                CommunicationTypeId = 1,
                CurrentStatusId = 1,
                CreatedByUserId = 1,
                IsActive = true,
                CreatedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow
            };

            var status2 = new GlobalStatus 
            { 
                Id = 2, 
                StatusCode = "Printed",
                DisplayName = "Printed",
                Description = "Document printed",
                Phase = StatusPhase.Production
            };
            _context.GlobalStatuses.Add(status2);

            var comm2 = new Communication
            {
                Id = 2,
                Title = "Status 2",
                MemberId = 1,
                CommunicationTypeId = 1,
                CurrentStatusId = 2,
                CreatedByUserId = 1,
                IsActive = true,
                CreatedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow
            };

            _context.Communications.AddRange(comm1, comm2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByStatusAsync(1);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Status 1"));
        }

        #endregion

        #region GetByTypeId Tests

        [Test]
        public async Task GetByTypeId_ReturnsFilteredResults()
        {
            // Arrange
            var type2 = new CommunicationType 
            { 
                Id = 2, 
                TypeCode = "ID_CARD",
                DisplayName = "ID Card",
                Description = "Member ID Card",
                IsActive = true
            };
            _context.CommunicationTypes.Add(type2);

            var comm1 = new Communication
            {
                Id = 1,
                Title = "EOB Doc",
                MemberId = 1,
                CommunicationTypeId = 1,
                CurrentStatusId = 1,
                CreatedByUserId = 1,
                IsActive = true,
                CreatedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow
            };

            var comm2 = new Communication
            {
                Id = 2,
                Title = "ID Card",
                MemberId = 1,
                CommunicationTypeId = 2,
                CurrentStatusId = 1,
                CreatedByUserId = 1,
                IsActive = true,
                CreatedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow
            };

            _context.Communications.AddRange(comm1, comm2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByTypeId(1);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("EOB Doc"));
        }

        #endregion
    }
}