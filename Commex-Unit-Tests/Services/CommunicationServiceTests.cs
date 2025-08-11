using Microsoft.Extensions.Logging;
using Moq;
using TSG_Commex_BE.Models.Domain;
using TSG_Commex_BE.Models.Enums;
using TSG_Commex_BE.Repositories.Interfaces;
using TSG_Commex_BE.Services.Implementations;
using TSG_Commex_Shared.DTOs;
using TSG_Commex_Shared.DTOs.Request;
using CommunicationStatusHistory = TSG_Commex_BE.Models.Domain.CommunicationStatusHistory;

namespace Commex_Unit_Tests.Services;

[TestFixture]
public class CommunicationServiceTests
{
    private Mock<ICommunicationRepository> _mockCommunicationRepo;
    private Mock<ICommunicationTypeStatusRepository> _mockTypeStatusRepo;
    private Mock<ICommunicationTypeRepository> _mockTypeRepo;
    private Mock<IGlobalStatusRepository> _mockStatusRepo;
    private Mock<IMemberRepository> _mockMemberRepo;
    private Mock<ILogger<CommunicationService>> _mockLogger;
    private CommunicationService _service;

    [SetUp]
    public void Setup()
    {
        _mockCommunicationRepo = new Mock<ICommunicationRepository>();
        _mockTypeStatusRepo = new Mock<ICommunicationTypeStatusRepository>();
        _mockTypeRepo = new Mock<ICommunicationTypeRepository>();
        _mockStatusRepo = new Mock<IGlobalStatusRepository>();
        _mockMemberRepo = new Mock<IMemberRepository>();
        _mockLogger = new Mock<ILogger<CommunicationService>>();
        
        _service = new CommunicationService(
            _mockCommunicationRepo.Object,
            _mockTypeStatusRepo.Object,
            _mockTypeRepo.Object,
            _mockStatusRepo.Object,
            _mockMemberRepo.Object,
            _mockLogger.Object
        );
    }

    #region GetAllCommunicationsAsync Tests

    [Test]
    public async Task GetAllCommunicationsAsync_ReturnsEmptyList_WhenNoCommunications()
    {
        // Arrange
        _mockCommunicationRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Communication>());

        // Act
        var result = await _service.GetAllCommunicationsAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetAllCommunicationsAsync_ReturnsMappedCommunications_WhenCommunicationsExist()
    {
        // Arrange
        var communications = new List<Communication>
        {
            new Communication
            {
                Id = 1,
                Title = "EOB Document",
                CommunicationTypeId = 1,
                CommunicationType = new CommunicationType 
                { 
                    Id = 1, 
                    TypeCode = "EOB", 
                    DisplayName = "Explanation of Benefits",
                    Description = "EOB documents"
                },
                CurrentStatusId = 1,
                CurrentStatus = new GlobalStatus 
                { 
                    Id = 1, 
                    StatusCode = "Created", 
                    DisplayName = "Created",
                    Description = "Document created",
                    Phase = StatusPhase.Creation
                },
                MemberId = 100,
                Member = new Member 
                { 
                    Id = 100, 
                    MemberId = "12345",
                    FirstName = "John", 
                    LastName = "Doe", 
                    Email = "john@example.com" 
                },
                CreatedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow,
                CreatedByUserId = 1,
                CreatedByUser = new User { Id = 1, Email = "admin@example.com", FirstName = "Admin", LastName = "User" }
            }
        };

        _mockCommunicationRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(communications);

        // Act
        var result = await _service.GetAllCommunicationsAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        var first = result.First();
        Assert.That(first.Id, Is.EqualTo(1));
        Assert.That(first.TypeCode, Is.EqualTo("EOB"));
        Assert.That(first.CurrentStatus, Is.EqualTo("Created"));
        Assert.That(first.MemberId, Is.EqualTo(100));
    }

    [Test]
    public async Task GetAllCommunicationsAsync_ThrowsException_WhenRepositoryFails()
    {
        // Arrange
        _mockCommunicationRepo.Setup(r => r.GetAllAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _service.GetAllCommunicationsAsync());
    }

    #endregion

    #region GetCommunicationByIdAsync Tests

    [Test]
    public async Task GetCommunicationByIdAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        _mockCommunicationRepo.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Communication?)null);

        // Act
        var result = await _service.GetCommunicationByIdAsync(999);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetCommunicationByIdAsync_ReturnsMappedCommunication_WhenFound()
    {
        // Arrange
        var communication = new Communication
        {
            Id = 1,
            Title = "EOB Document",
            CommunicationTypeId = 1,
            CommunicationType = new CommunicationType 
            { 
                Id = 1, 
                TypeCode = "EOB", 
                DisplayName = "Explanation of Benefits",
                Description = "EOB documents"
            },
            CurrentStatusId = 1,
            CurrentStatus = new GlobalStatus 
            { 
                Id = 1, 
                StatusCode = "Created", 
                DisplayName = "Created",
                Description = "Document created",
                Phase = StatusPhase.Creation
            },
            MemberId = 100,
            Member = new Member 
            { 
                Id = 100,
                MemberId = "12345",
                FirstName = "John", 
                LastName = "Doe", 
                Email = "john@example.com" 
            },
            CreatedUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow,
            CreatedByUserId = 1,
            CreatedByUser = new User { Id = 1, Email = "admin@example.com", FirstName = "Admin", LastName = "User" }
        };

        _mockCommunicationRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(communication);

        // Act
        var result = await _service.GetCommunicationByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(1));
        Assert.That(result.TypeCode, Is.EqualTo("EOB"));
        Assert.That(result.MemberId, Is.EqualTo(100));
    }

    #endregion

    #region CreateCommunicationAsync Tests

    [Test]
    public async Task CreateCommunicationAsync_CreatesWithProvidedStatus_WhenInitialStatusIdProvided()
    {
        // Arrange
        var request = new CreateCommunicationRequest
        {
            MemberId = 100,
            CommunicationTypeId = 1,
            Title = "New EOB",
            InitialStatusId = 2,  // Providing specific status
            CreatedByUserId = 1
        };

        var member = new Member 
        { 
            Id = 100,
            MemberId = "12345",
            FirstName = "John", 
            LastName = "Doe", 
            Email = "john@example.com" 
        };
        
        var commType = new CommunicationType 
        { 
            Id = 1, 
            TypeCode = "EOB", 
            DisplayName = "Explanation of Benefits",
            Description = "EOB documents"
        };
        
        var status = new GlobalStatus 
        { 
            Id = 2, 
            StatusCode = "Printed", 
            DisplayName = "Printed",
            Description = "Document printed",
            Phase = StatusPhase.Production
        };

        var user = new User { Id = 1, Email = "admin@example.com", FirstName = "Admin", LastName = "User" };

        _mockMemberRepo.Setup(r => r.GetByIdAsync(100))
            .ReturnsAsync(member);
        _mockTypeRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(commType);
        _mockStatusRepo.Setup(r => r.GetByIdAsync(2))
            .ReturnsAsync(status);

        var savedCommunication = new Communication
        {
            Id = 1,
            Title = "New EOB",
            MemberId = 100,
            Member = member,
            CommunicationTypeId = 1,
            CommunicationType = commType,
            CurrentStatusId = 2,
            CurrentStatus = status,
            CreatedUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow,
            CreatedByUserId = 1,
            CreatedByUser = user
        };

        _mockCommunicationRepo.Setup(r => r.CreateAsync(It.IsAny<Communication>()))
            .ReturnsAsync(savedCommunication);

        // Act
        var result = await _service.CreateCommunicationAsync(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.MemberId, Is.EqualTo(100));
        Assert.That(result.CurrentStatusId, Is.EqualTo(2));
        _mockCommunicationRepo.Verify(r => r.CreateAsync(It.IsAny<Communication>()), Times.Once);
    }

    [Test]
    public async Task CreateCommunicationAsync_UsesDefaultStatus_WhenInitialStatusIdNotProvided()
    {
        // Arrange
        var request = new CreateCommunicationRequest
        {
            MemberId = 100,
            CommunicationTypeId = 1,
            Title = "New EOB"
            // No InitialStatusId provided
        };

        var member = new Member 
        { 
            Id = 100,
            MemberId = "12345",
            FirstName = "John", 
            LastName = "Doe", 
            Email = "john@example.com" 
        };
        
        var commType = new CommunicationType 
        { 
            Id = 1, 
            TypeCode = "EOB", 
            DisplayName = "Explanation of Benefits",
            Description = "EOB documents"
        };
        
        var defaultStatus = new GlobalStatus 
        { 
            Id = 1, 
            StatusCode = "ReadyForRelease", 
            DisplayName = "Ready for Release",
            Description = "Ready to be released",
            Phase = StatusPhase.Creation
        };

        _mockMemberRepo.Setup(r => r.GetByIdAsync(100))
            .ReturnsAsync(member);
        _mockTypeRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(commType);
        _mockStatusRepo.Setup(r => r.GetByStatusCodeAsync("ReadyForRelease"))
            .ReturnsAsync(defaultStatus);
        _mockStatusRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(defaultStatus);

        var savedCommunication = new Communication
        {
            Id = 1,
            Title = "New EOB",
            MemberId = 100,
            Member = member,
            CommunicationTypeId = 1,
            CommunicationType = commType,
            CurrentStatusId = 1,
            CurrentStatus = defaultStatus,
            CreatedUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow
        };

        _mockCommunicationRepo.Setup(r => r.CreateAsync(It.IsAny<Communication>()))
            .ReturnsAsync(savedCommunication);

        // Act
        var result = await _service.CreateCommunicationAsync(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.CurrentStatusId, Is.EqualTo(1));
        Assert.That(result.CurrentStatus, Is.EqualTo("ReadyForRelease"));
        _mockStatusRepo.Verify(r => r.GetByStatusCodeAsync("ReadyForRelease"), Times.Once);
    }

    [Test]
    public async Task CreateCommunicationAsync_ThrowsException_WhenMemberNotFound()
    {
        // Arrange
        var request = new CreateCommunicationRequest
        {
            MemberId = 999,
            CommunicationTypeId = 1,
            Title = "New EOB"
        };

        _mockMemberRepo.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Member?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await _service.CreateCommunicationAsync(request));
        Assert.That(ex!.Message, Does.Contain("Member with ID 999 not found"));
    }

    [Test]
    public async Task CreateCommunicationAsync_ThrowsException_WhenTypeNotFound()
    {
        // Arrange
        var request = new CreateCommunicationRequest
        {
            MemberId = 100,
            CommunicationTypeId = 999,
            Title = "New EOB"
        };

        var member = new Member 
        { 
            Id = 100,
            MemberId = "12345",
            FirstName = "John", 
            LastName = "Doe", 
            Email = "john@example.com" 
        };

        _mockMemberRepo.Setup(r => r.GetByIdAsync(100))
            .ReturnsAsync(member);
        _mockTypeRepo.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((CommunicationType?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await _service.CreateCommunicationAsync(request));
        Assert.That(ex!.Message, Does.Contain("Communication type with ID 999 not found"));
    }

    #endregion

    #region UpdateCommunicationAsync Tests

    [Test]
    public async Task UpdateCommunicationAsync_ReturnsTrue_WhenSuccessful()
    {
        // Arrange
        var existingComm = new Communication
        {
            Id = 1,
            Title = "Old Title",
            CurrentStatusId = 1,
            MemberId = 100,
            CommunicationTypeId = 1,
            LastUpdatedUtc = DateTime.UtcNow.AddDays(-1)
        };

        var request = new UpdateCommunicationRequest
        {
            Title = "New Title",
            CurrentStatusId = 2,
            UpdatedByUserId = 1
        };

        _mockCommunicationRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingComm);
        _mockCommunicationRepo.Setup(r => r.UpdateAsync(It.IsAny<Communication>()))
            .Returns(Task.FromResult(true));

        // Act
        var result = await _service.UpdateCommunicationAsync(1, request);

        // Assert
        Assert.That(result, Is.True);
        _mockCommunicationRepo.Verify(r => r.UpdateAsync(It.IsAny<Communication>()), Times.Once);
    }

    [Test]
    public async Task UpdateCommunicationAsync_ReturnsFalse_WhenNotFound()
    {
        // Arrange
        var request = new UpdateCommunicationRequest { Title = "New Title" };
        _mockCommunicationRepo.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Communication?)null);

        // Act
        var result = await _service.UpdateCommunicationAsync(999, request);

        // Assert
        Assert.That(result, Is.False);
        _mockCommunicationRepo.Verify(r => r.UpdateAsync(It.IsAny<Communication>()), Times.Never);
    }

    #endregion

    #region DeleteCommunicationAsync Tests

    [Test]
    public async Task DeleteCommunicationAsync_ReturnsTrue_WhenSuccessful()
    {
        // Arrange
        var communication = new Communication { Id = 1, Title = "Test" };
        _mockCommunicationRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(communication);
        _mockCommunicationRepo.Setup(r => r.DeleteAsync(1))
            .Returns(Task.FromResult(true));

        // Act
        var result = await _service.DeleteCommunicationAsync(1);

        // Assert
        Assert.That(result, Is.True);
        _mockCommunicationRepo.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Test]
    public async Task DeleteCommunicationAsync_ReturnsFalse_WhenNotFound()
    {
        // Arrange
        _mockCommunicationRepo.Setup(r => r.DeleteAsync(999))
            .Returns(Task.FromResult(false));

        // Act
        var result = await _service.DeleteCommunicationAsync(999);

        // Assert
        Assert.That(result, Is.False);
    }

    #endregion

    #region UpdateStatusAsync Tests

    [Test]
    public async Task UpdateStatusAsync_ReturnsTrue_WhenSuccessful()
    {
        // Arrange
        var communication = new Communication
        {
            Id = 1,
            Title = "Test Communication",
            CurrentStatusId = 1,
            MemberId = 100,
            CommunicationTypeId = 1,
            StatusHistory = new List<CommunicationStatusHistory>()
        };

        var newStatus = new GlobalStatus 
        { 
            Id = 2, 
            StatusCode = "Printed", 
            DisplayName = "Printed",
            Description = "Document printed",
            Phase = StatusPhase.Production
        };

        _mockCommunicationRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(communication);
        _mockStatusRepo.Setup(r => r.GetByIdAsync(2))
            .ReturnsAsync(newStatus);
        _mockCommunicationRepo.Setup(r => r.UpdateAsync(It.IsAny<Communication>()))
            .Returns(Task.FromResult(true));

        // Act
        var result = await _service.UpdateStatusAsync(1, 2, 1);

        // Assert
        Assert.That(result, Is.True);
        _mockCommunicationRepo.Verify(r => r.UpdateAsync(It.IsAny<Communication>()), Times.Once);
    }

    [Test]
    public async Task UpdateStatusAsync_ReturnsFalse_WhenCommunicationNotFound()
    {
        // Arrange
        _mockCommunicationRepo.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Communication?)null);

        // Act
        var result = await _service.UpdateStatusAsync(999, 2, 1);
        
        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateStatusAsync_ReturnsFalse_WhenStatusNotFound()
    {
        // Arrange
        var communication = new Communication
        {
            Id = 1,
            Title = "Test Communication",
            CurrentStatusId = 1,
            MemberId = 100,
            CommunicationTypeId = 1
        };

        _mockCommunicationRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(communication);
        _mockStatusRepo.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((GlobalStatus?)null);

        // Act
        var result = await _service.UpdateStatusAsync(1, 999, 1);
        
        // Assert
        Assert.That(result, Is.False);
    }

    #endregion
}