using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TSG_Commex_BE.Data;
using TSG_Commex_BE.Models.Domain;
using TSG_Commex_BE.Repositories.Implementations;
using TSG_Commex_BE.Repositories.Interfaces;

namespace Commex_Unit_Tests.Repositories
{
    [TestFixture]
    public class MemberRepositoryTests
    {
        private ApplicationDbContext _context;
        private IMemberRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            
            _context = new ApplicationDbContext(options);
            _repository = new MemberRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        #region GetAllAsync Tests

        [Test]
        public async Task GetAllAsync_ReturnsAllMembers()
        {
            // Arrange
            var member1 = new Member
            {
                Id = 1,
                MemberId = "M12345",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                DateOfBirth = new DateTime(1980, 1, 1),
                IsActive = true,
                CreatedUtc = DateTime.UtcNow
            };

            var member2 = new Member
            {
                Id = 2,
                MemberId = "M67890",
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                IsActive = false,
                CreatedUtc = DateTime.UtcNow
            };

            _context.Members.AddRange(member1, member2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().MemberId, Is.EqualTo("M12345")); // Ordered by LastName
        }

        #endregion

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsMember()
        {
            // Arrange
            var member = new Member
            {
                Id = 1,
                MemberId = "M12345",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                IsActive = true,
                CreatedUtc = DateTime.UtcNow
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.MemberId, Is.EqualTo("M12345"));
            Assert.That(result.Email, Is.EqualTo("john@example.com"));
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
        public async Task GetByIdAsync_InactiveMember_StillReturnsMember()
        {
            // Arrange
            var member = new Member
            {
                Id = 1,
                MemberId = "M12345",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                IsActive = false,
                CreatedUtc = DateTime.UtcNow
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsActive, Is.False);
        }

        #endregion

        #region GetByMemberIdAsync Tests

        [Test]
        public async Task GetByMemberIdAsync_ExistingMemberId_ReturnsMember()
        {
            // Arrange
            var member = new Member
            {
                Id = 1,
                MemberId = "M12345",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                IsActive = true,
                CreatedUtc = DateTime.UtcNow
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByMemberIdAsync("M12345");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetByMemberIdAsync_NonExistingMemberId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByMemberIdAsync("INVALID");

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region CreateAsync Tests

        [Test]
        public async Task CreateAsync_ValidMember_SavesToDatabase()
        {
            // Arrange
            var member = new Member
            {
                MemberId = "M99999",
                FirstName = "New",
                LastName = "Member",
                Email = "new@example.com",
                DateOfBirth = new DateTime(1990, 6, 15),
                IsActive = true
            };

            // Act
            var result = await _repository.CreateAsync(member);

            // Assert
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.CreatedUtc, Is.Not.EqualTo(DateTime.MinValue));
            
            var saved = await _context.Members.FindAsync(result.Id);
            Assert.That(saved, Is.Not.Null);
            Assert.That(saved.MemberId, Is.EqualTo("M99999"));
        }

        #endregion

        #region UpdateAsync Tests

        [Test]
        public async Task UpdateAsync_ExistingMember_UpdatesDatabase()
        {
            // Arrange
            var member = new Member
            {
                Id = 1,
                MemberId = "M12345",
                FirstName = "Original",
                LastName = "Name",
                Email = "original@example.com",
                IsActive = true,
                CreatedUtc = DateTime.UtcNow.AddDays(-1)
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();
            _context.Entry(member).State = EntityState.Detached;

            // Act
            member.FirstName = "Updated";
            member.LastName = "Person";
            member.Email = "updated@example.com";
            await _repository.UpdateAsync(member);

            // Assert
            var updated = await _context.Members.FindAsync(1);
            Assert.That(updated.FirstName, Is.EqualTo("Updated"));
            Assert.That(updated.LastName, Is.EqualTo("Person"));
            Assert.That(updated.Email, Is.EqualTo("updated@example.com"));
        }

        #endregion

        #region DeleteAsync Tests

        [Test]
        public async Task DeleteAsync_ExistingId_SoftDeletes()
        {
            // Arrange
            var member = new Member
            {
                Id = 1,
                MemberId = "M12345",
                FirstName = "To",
                LastName = "Delete",
                Email = "delete@example.com",
                IsActive = true,
                CreatedUtc = DateTime.UtcNow
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(1);

            // Assert
            var deleted = await _context.Members.FindAsync(1);
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

        #region GetActiveAsync Tests

        [Test]
        public async Task GetActiveAsync_ReturnsOnlyActiveMembers()
        {
            // Arrange
            var active1 = new Member
            {
                Id = 1,
                MemberId = "M11111",
                FirstName = "Active",
                LastName = "One",
                Email = "active1@example.com",
                IsActive = true,
                CreatedUtc = DateTime.UtcNow
            };

            var inactive = new Member
            {
                Id = 2,
                MemberId = "M22222",
                FirstName = "Inactive",
                LastName = "Member",
                Email = "inactive@example.com",
                IsActive = false,
                CreatedUtc = DateTime.UtcNow
            };

            var active2 = new Member
            {
                Id = 3,
                MemberId = "M33333",
                FirstName = "Active",
                LastName = "Two",
                Email = "active2@example.com",
                IsActive = true,
                CreatedUtc = DateTime.UtcNow
            };

            _context.Members.AddRange(active1, inactive, active2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetActiveAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(m => m.IsActive), Is.True);
        }

        #endregion
    }
}