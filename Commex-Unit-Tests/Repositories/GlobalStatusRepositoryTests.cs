using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TSG_Commex_BE.Data;
using TSG_Commex_BE.Models.Domain;
using TSG_Commex_BE.Models.Enums;
using TSG_Commex_BE.Repositories.Implementations;
using TSG_Commex_BE.Repositories.Interfaces;

namespace Commex_Unit_Tests.Repositories
{
    [TestFixture]
    public class GlobalStatusRepositoryTests
    {
        private ApplicationDbContext _context;
        private IGlobalStatusRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            
            _context = new ApplicationDbContext(options);
            _repository = new GlobalStatusRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        #region GetAllAsync Tests

        [Test]
        public async Task GetAllAsync_ReturnsAllActiveStatuses()
        {
            // Arrange
            var activeStatus = new GlobalStatus
            {
                Id = 1,
                StatusCode = "ReadyForRelease",
                DisplayName = "Ready for Release",
                Description = "Document ready for release",
                Phase = StatusPhase.Creation,
                IsActive = true
            };

            var inactiveStatus = new GlobalStatus
            {
                Id = 2,
                StatusCode = "Obsolete",
                DisplayName = "Obsolete",
                Description = "No longer used",
                Phase = StatusPhase.Terminal,
                IsActive = false
            };

            _context.GlobalStatuses.AddRange(activeStatus, inactiveStatus);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().StatusCode, Is.EqualTo("ReadyForRelease"));
        }

        #endregion

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsStatus()
        {
            // Arrange
            var status = new GlobalStatus
            {
                Id = 1,
                StatusCode = "Printed",
                DisplayName = "Printed",
                Description = "Document has been printed",
                Phase = StatusPhase.Production,
                IsActive = true
            };

            _context.GlobalStatuses.Add(status);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo("Printed"));
        }

        [Test]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region GetByStatusCodeAsync Tests

        [Test]
        public async Task GetByStatusCodeAsync_ExistingCode_ReturnsStatus()
        {
            // Arrange
            var status = new GlobalStatus
            {
                Id = 1,
                StatusCode = "Delivered",
                DisplayName = "Delivered",
                Description = "Successfully delivered",
                Phase = StatusPhase.Logistics,
                IsActive = true
            };

            _context.GlobalStatuses.Add(status);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByStatusCodeAsync("Delivered");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetByStatusCodeAsync_NonExistingCode_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByStatusCodeAsync("NonExistent");

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region CreateAsync Tests

        [Test]
        public async Task CreateAsync_ValidStatus_SavesToDatabase()
        {
            // Arrange
            var status = new GlobalStatus
            {
                StatusCode = "NewStatus",
                DisplayName = "New Status",
                Description = "A new status",
                Phase = StatusPhase.Creation,
                IsActive = true
            };

            // Act
            var result = await _repository.CreateAsync(status);

            // Assert
            Assert.That(result.Id, Is.GreaterThan(0));
            
            var saved = await _context.GlobalStatuses.FindAsync(result.Id);
            Assert.That(saved, Is.Not.Null);
            Assert.That(saved.StatusCode, Is.EqualTo("NewStatus"));
        }

        #endregion

        #region UpdateAsync Tests

        [Test]
        public async Task UpdateAsync_ExistingStatus_UpdatesDatabase()
        {
            // Arrange
            var status = new GlobalStatus
            {
                Id = 1,
                StatusCode = "TestStatus",
                DisplayName = "Original Name",
                Description = "Original description",
                Phase = StatusPhase.Creation,
                IsActive = true
            };

            _context.GlobalStatuses.Add(status);
            await _context.SaveChangesAsync();
            _context.Entry(status).State = EntityState.Detached;

            // Act
            status.DisplayName = "Updated Name";
            status.Description = "Updated description";
            await _repository.UpdateAsync(status);

            // Assert
            var updated = await _context.GlobalStatuses.FindAsync(1);
            Assert.That(updated.DisplayName, Is.EqualTo("Updated Name"));
            Assert.That(updated.Description, Is.EqualTo("Updated description"));
        }

        #endregion

        #region DeleteAsync Tests

        [Test]
        public async Task DeleteAsync_ExistingId_SoftDeletes()
        {
            // Arrange
            var status = new GlobalStatus
            {
                Id = 1,
                StatusCode = "ToDelete",
                DisplayName = "To Delete",
                Description = "Will be deleted",
                Phase = StatusPhase.Terminal,
                IsActive = true
            };

            _context.GlobalStatuses.Add(status);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(1);

            // Assert
            var deleted = await _context.GlobalStatuses.FindAsync(1);
            Assert.That(deleted, Is.Not.Null);
            Assert.That(deleted.IsActive, Is.False);
        }

        #endregion

        #region GetByPhaseAsync Tests

        [Test]
        public async Task GetByPhaseAsync_ReturnsFilteredStatuses()
        {
            // Arrange
            var creationStatus = new GlobalStatus
            {
                Id = 1,
                StatusCode = "ReadyForRelease",
                DisplayName = "Ready for Release",
                Description = "Ready",
                Phase = StatusPhase.Creation,
                IsActive = true
            };

            var productionStatus = new GlobalStatus
            {
                Id = 2,
                StatusCode = "Printed",
                DisplayName = "Printed",
                Description = "Printed",
                Phase = StatusPhase.Production,
                IsActive = true
            };

            _context.GlobalStatuses.AddRange(creationStatus, productionStatus);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByPhaseAsync("Creation");

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().StatusCode, Is.EqualTo("ReadyForRelease"));
        }

        #endregion
    }
}