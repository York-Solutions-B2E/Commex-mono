using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TSG_Commex_BE.Controllers;
using TSG_Commex_BE.Services.Interfaces;
using TSG_Commex_Shared.DTOs;
using TSG_Commex_Shared.DTOs.Request;

namespace Commex_Unit_Tests.Controllers;

[TestFixture]
public class CommunicationsControllerTests
{
    private Mock<ICommunicationService> _mockService;
    private Mock<ILogger<CommunicationsController>> _mockLogger;
    private CommunicationsController _controller;

    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<ICommunicationService>();
        _mockLogger = new Mock<ILogger<CommunicationsController>>();
        _controller = new CommunicationsController(_mockService.Object, _mockLogger.Object);
    }

    #region GetCommunications Tests

    [Test]
    public async Task GetCommunications_WhenSuccess_ReturnsOkWithCommunications()
    {
        // Arrange
        var expectedCommunications = new List<CommunicationResponse>
        {
            new() 
            { 
                Id = 1, 
                MemberId = 12345,  // Integer member ID
                MemberName = "John Doe",
                CommunicationTypeId = 1,
                TypeCode = "EOB",
                CurrentStatusId = 1,
                CurrentStatus = "Created",
                Subject = "Explanation of Benefits",
                CreatedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow,
                CreatedByUserName = "system",
                RecipientInfo = "john.doe@example.com"
            },
            new() 
            { 
                Id = 2, 
                MemberId = 67890,  // Integer member ID
                MemberName = "Jane Smith",
                CommunicationTypeId = 2,
                TypeCode = "ID_CARD",
                CurrentStatusId = 3,
                CurrentStatus = "Printed",
                Subject = "Member ID Card",
                CreatedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow,
                CreatedByUserName = "system",
                RecipientInfo = "jane.smith@example.com"
            }
        };
        _mockService.Setup(s => s.GetAllCommunicationsAsync())
            .ReturnsAsync(expectedCommunications);
        var request = new GetCommunicationRequest();

        // Act
        var result = await _controller.GetCommunications(request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var communications = okResult.Value as IEnumerable<CommunicationResponse>;
        Assert.That(communications?.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetCommunications_WhenServiceThrows_Returns500()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllCommunicationsAsync())
            .ThrowsAsync(new Exception("Database error"));
        var request = new GetCommunicationRequest();

        // Act
        var result = await _controller.GetCommunications(request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)result.Result!;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
    }

    #endregion

    #region GetCommunication Tests

    [Test]
    public async Task GetCommunication_WhenFound_ReturnsOkWithCommunication()
    {
        // Arrange
        var expectedCommunication = new CommunicationResponse 
        { 
            Id = 1, 
            MemberId = 12345,  // Integer member ID
            MemberName = "John Doe",
            CommunicationTypeId = 1,
            TypeCode = "EOB",
            CurrentStatusId = 1,
            CurrentStatus = "Created",
            Subject = "Explanation of Benefits",
            CreatedUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow,
            CreatedByUserName = "system",
            RecipientInfo = "john.doe@example.com"
        };
        _mockService.Setup(s => s.GetCommunicationByIdAsync(1))
            .ReturnsAsync(expectedCommunication);

        // Act
        var result = await _controller.GetCommunication(1);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var communication = okResult.Value as CommunicationResponse;
        Assert.That(communication?.Id, Is.EqualTo(1));
        Assert.That(communication?.MemberId, Is.EqualTo(12345));
    }

    [Test]
    public async Task GetCommunication_WhenNotFound_Returns404()
    {
        // Arrange
        _mockService.Setup(s => s.GetCommunicationByIdAsync(999))
            .ReturnsAsync((CommunicationResponse?)null);

        // Act
        var result = await _controller.GetCommunication(999);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task GetCommunication_WhenServiceThrows_Returns500()
    {
        // Arrange
        _mockService.Setup(s => s.GetCommunicationByIdAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetCommunication(1);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)result.Result!;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
    }

    #endregion

    #region CreateCommunication Tests

    [Test]
    public async Task CreateCommunication_WhenValid_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new CreateCommunicationRequest 
        { 
            MemberId = 12345,  // Integer member ID
            CommunicationTypeId = 1,
            Title = "EOB for Claim #123",
            SourceFileUrl = "https://storage.example.com/eob123.pdf",
            InitialStatusId = 1,
            CreatedByUserId = 1
        };
        var expectedResponse = new CommunicationResponse 
        { 
            Id = 1, 
            MemberId = 12345,
            MemberName = "John Doe",
            CommunicationTypeId = 1,
            TypeCode = "EOB",
            CurrentStatusId = 1,
            CurrentStatus = "Created",
            Subject = "EOB for Claim #123",
            CreatedUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow,
            CreatedByUserName = "system",
            RecipientInfo = "john.doe@example.com"
        };
        _mockService.Setup(s => s.CreateCommunicationAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.CreateCommunication(request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var createdResult = (CreatedAtActionResult)result.Result!;
        Assert.That(createdResult.ActionName, Is.EqualTo(nameof(_controller.GetCommunication)));
        Assert.That(createdResult.RouteValues?["id"], Is.EqualTo(1));
    }

    [Test]
    public async Task CreateCommunication_WhenServiceThrows_Returns500()
    {
        // Arrange
        var request = new CreateCommunicationRequest 
        { 
            MemberId = 12345,
            CommunicationTypeId = 1,
            Title = "Test Communication"
        };
        _mockService.Setup(s => s.CreateCommunicationAsync(It.IsAny<CreateCommunicationRequest>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateCommunication(request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)result.Result!;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
    }

    #endregion

    #region UpdateCommunication Tests

    [Test]
    public async Task UpdateCommunication_WhenSuccess_ReturnsNoContent()
    {
        // Arrange
        var request = new UpdateCommunicationRequest 
        { 
            CurrentStatusId = 3,  // Using integer status ID
            Title = "Updated Title",
            UpdatedByUserId = 1
        };
        _mockService.Setup(s => s.UpdateCommunicationAsync(1, request))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateCommunication(1, request);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task UpdateCommunication_WhenNotFound_Returns404()
    {
        // Arrange
        var request = new UpdateCommunicationRequest 
        { 
            CurrentStatusId = 3,
            Title = "Updated Title"
        };
        _mockService.Setup(s => s.UpdateCommunicationAsync(999, request))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateCommunication(999, request);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task UpdateCommunication_WhenServiceThrows_Returns500()
    {
        // Arrange
        var request = new UpdateCommunicationRequest 
        { 
            CurrentStatusId = 3 
        };
        _mockService.Setup(s => s.UpdateCommunicationAsync(It.IsAny<int>(), It.IsAny<UpdateCommunicationRequest>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdateCommunication(1, request);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)result;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
    }

    #endregion

    #region DeleteCommunication Tests

    [Test]
    public async Task DeleteCommunication_WhenSuccess_ReturnsNoContent()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteCommunicationAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteCommunication(1);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task DeleteCommunication_WhenNotFound_Returns404()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteCommunicationAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteCommunication(999);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task DeleteCommunication_WhenServiceThrows_Returns500()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteCommunicationAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.DeleteCommunication(1);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = (ObjectResult)result;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
    }

    #endregion
}