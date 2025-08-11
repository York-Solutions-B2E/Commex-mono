using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TSG_Commex_BE.Controllers;
using TSG_Commex_BE.Services.Interfaces;
using TSG_Commex_Shared.DTOs.Response;
using TSG_Commex_Shared.DTOs.Request;

namespace Commex_Unit_Tests;

[TestFixture]
public class GlobalStatusesControllerTests
{
    private Mock<IGlobalStatusService> _mockService;
    private Mock<ILogger<GlobalStatusesController>> _mockLogger;
    private GlobalStatusesController _controller;

    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<IGlobalStatusService>();
        _mockLogger = new Mock<ILogger<GlobalStatusesController>>();
        _controller = new GlobalStatusesController(_mockService.Object, _mockLogger.Object);
    }

    [Test]
    public async Task GetAllStatuses_ReturnsOkResult_WithListOfStatuses()
    {
        // Arrange
        var expectedStatuses = new List<GlobalStatusResponse>
        {
            new GlobalStatusResponse 
            { 
                Id = 1,
                StatusCode = "Created",
                DisplayName = "Created",
                Description = "Initial creation",
                Phase = "Creation"
            },
            new GlobalStatusResponse 
            { 
                Id = 2,
                StatusCode = "ReadyForRelease",
                DisplayName = "Ready for Release",
                Description = "Ready to be released",
                Phase = "Creation"
            }
        };

        _mockService.Setup(s => s.GetAllStatusesAsync())
            .ReturnsAsync(expectedStatuses);

        // Act
        var result = await _controller.GetAllStatuses();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult?.Value, Is.InstanceOf<IEnumerable<GlobalStatusResponse>>());
        
        var statuses = okResult?.Value as IEnumerable<GlobalStatusResponse>;
        Assert.That(statuses?.Count(), Is.EqualTo(2));
        
        _mockService.Verify(s => s.GetAllStatusesAsync(), Times.Once);
    }

    [Test]
    public async Task GetAllStatuses_WhenServiceThrows_Returns500()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllStatusesAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAllStatuses();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult?.StatusCode, Is.EqualTo(500));
        
        _mockService.Verify(s => s.GetAllStatusesAsync(), Times.Once);
    }

    [Test]
    public async Task GetStatusById_ExistingId_ReturnsOkResult()
    {
        // Arrange
        var expectedStatus = new GlobalStatusResponse 
        { 
            Id = 1,
            StatusCode = "Created",
            DisplayName = "Created",
            Description = "Initial creation",
            Phase = "Creation"
        };

        _mockService.Setup(s => s.GetStatusByIdAsync(1))
            .ReturnsAsync(expectedStatus);

        // Act
        var result = await _controller.GetStatusById(1);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var status = okResult?.Value as GlobalStatusResponse;
        
        Assert.That(status, Is.Not.Null);
        Assert.That(status?.Id, Is.EqualTo(1));
        Assert.That(status?.StatusCode, Is.EqualTo("Created"));
        
        _mockService.Verify(s => s.GetStatusByIdAsync(1), Times.Once);
    }

    [Test]
    public async Task GetStatusById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetStatusByIdAsync(999))
            .ReturnsAsync((GlobalStatusResponse?)null);

        // Act
        var result = await _controller.GetStatusById(999);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        _mockService.Verify(s => s.GetStatusByIdAsync(999), Times.Once);
    }

    [Test]
    public async Task CreateStatus_ValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new CreateGlobalStatusRequest
        {
            StatusCode = "PROCESSING",
            DisplayName = "Processing",
            Description = "Document is being processed",
            Phase = "Production"
        };

        var expectedResponse = new GlobalStatusResponse
        {
            Id = 10,
            StatusCode = request.StatusCode,
            DisplayName = request.DisplayName,
            Description = request.Description,
            Phase = request.Phase
        };

        _mockService.Setup(s => s.CreateStatusAsync(It.IsAny<CreateGlobalStatusRequest>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.CreateStatus(request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var createdResult = result.Result as CreatedAtActionResult;
        
        Assert.That(createdResult?.ActionName, Is.EqualTo("GetStatusById"));
        Assert.That(createdResult?.RouteValues?["id"], Is.EqualTo(10));
        
        var status = createdResult?.Value as GlobalStatusResponse;
        Assert.That(status?.StatusCode, Is.EqualTo("PROCESSING"));
        
        _mockService.Verify(s => s.CreateStatusAsync(It.IsAny<CreateGlobalStatusRequest>()), Times.Once);
    }

    [Test]
    public async Task CreateStatus_DuplicateCode_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateGlobalStatusRequest
        {
            StatusCode = "Created",
            DisplayName = "Created",
            Description = "Already exists",
            Phase = "Creation"
        };

        _mockService.Setup(s => s.CreateStatusAsync(It.IsAny<CreateGlobalStatusRequest>()))
            .ThrowsAsync(new InvalidOperationException("Status code already exists"));

        // Act
        var result = await _controller.CreateStatus(request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequest = result.Result as BadRequestObjectResult;
        
        Assert.That(badRequest?.Value?.ToString(), Does.Contain("Status code already exists"));
        _mockService.Verify(s => s.CreateStatusAsync(It.IsAny<CreateGlobalStatusRequest>()), Times.Once);
    }

    [Test]
    public async Task UpdateStatus_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var request = new UpdateGlobalStatusRequest
        {
            DisplayName = "Updated Display Name",
            Description = "Updated Description"
        };

        _mockService.Setup(s => s.UpdateStatusAsync(1, It.IsAny<UpdateGlobalStatusRequest>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateStatus(1, request);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        _mockService.Verify(s => s.UpdateStatusAsync(1, It.IsAny<UpdateGlobalStatusRequest>()), Times.Once);
    }

    [Test]
    public async Task UpdateStatus_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var request = new UpdateGlobalStatusRequest
        {
            DisplayName = "Updated Display Name"
        };

        _mockService.Setup(s => s.UpdateStatusAsync(999, It.IsAny<UpdateGlobalStatusRequest>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateStatus(999, request);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        _mockService.Verify(s => s.UpdateStatusAsync(999, It.IsAny<UpdateGlobalStatusRequest>()), Times.Once);
    }

    [Test]
    public async Task DeleteStatus_ExistingId_ReturnsNoContent()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteStatusAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteStatus(1);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        _mockService.Verify(s => s.DeleteStatusAsync(1), Times.Once);
    }

    [Test]
    public async Task DeleteStatus_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteStatusAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteStatus(999);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        _mockService.Verify(s => s.DeleteStatusAsync(999), Times.Once);
    }
}