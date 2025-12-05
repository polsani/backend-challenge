using System.Text;
using Challenge.Api.ErrorHandling;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Challenge.Api.Tests.ErrorHandling;

public class ErrorHandlingMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<ILogger<ErrorHandlingMiddleware>> _loggerMock;
    private readonly ErrorHandlingMiddleware _middleware;
    private readonly DefaultHttpContext _context;

    public ErrorHandlingMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
        _middleware = new ErrorHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
        _context = new DefaultHttpContext();
        _context.Response.Body = new MemoryStream();
    }

    [Fact(DisplayName = "Should return 400 BadRequest for ApplicationException")]
    public async Task ShouldReturn400BadRequestForApplicationException()
    {
        // Arrange
        var exception = new ApplicationException("Test application exception");
        _nextMock.Setup(n => n(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        _context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        _context.Response.ContentType.Should().Be("application/json");
        
        var response = await GetResponseBody();
        response.Should().Contain("Test application exception");
    }

    [Fact(DisplayName = "Should return 404 NotFound for KeyNotFoundException")]
    public async Task ShouldReturn404NotFoundForKeyNotFoundException()
    {
        // Arrange
        var exception = new KeyNotFoundException("Key not found");
        _nextMock.Setup(n => n(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        _context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        _context.Response.ContentType.Should().Be("application/json");
        
        var response = await GetResponseBody();
        response.Should().Contain("Resource not found");
    }

    [Fact(DisplayName = "Should return 401 Unauthorized for UnauthorizedAccessException")]
    public async Task ShouldReturn401UnauthorizedForUnauthorizedAccessException()
    {
        // Arrange
        var exception = new UnauthorizedAccessException("Unauthorized");
        _nextMock.Setup(n => n(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        _context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        _context.Response.ContentType.Should().Be("application/json");
        
        var response = await GetResponseBody();
        response.Should().Contain("Unauthorized access");
    }

    [Fact(DisplayName = "Should return 400 BadRequest for ArgumentException")]
    public async Task ShouldReturn400BadRequestForArgumentException()
    {
        // Arrange
        var exception = new ArgumentException("Invalid argument");
        _nextMock.Setup(n => n(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        _context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        _context.Response.ContentType.Should().Be("application/json");
        
        var response = await GetResponseBody();
        response.Should().Contain("Invalid argument");
    }

    [Fact(DisplayName = "Should return 500 InternalServerError for generic Exception")]
    public async Task ShouldReturn500InternalServerErrorForGenericException()
    {
        // Arrange
        var exception = new Exception("Generic error");
        _nextMock.Setup(n => n(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        _context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        _context.Response.ContentType.Should().Be("application/json");
        
        var response = await GetResponseBody();
        response.Should().Contain("An internal server error occurred");
    }

    [Fact(DisplayName = "Should include inner exception details in response")]
    public async Task ShouldIncludeInnerExceptionDetailsInResponse()
    {
        // Arrange
        var innerException = new Exception("Inner exception");
        var exception = new ApplicationException("Outer exception", innerException);
        _nextMock.Setup(n => n(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        var response = await GetResponseBody();
        response.Should().Contain("Inner exception");
    }

    [Fact(DisplayName = "Should log error when exception occurs")]
    public async Task ShouldLogErrorWhenExceptionOccurs()
    {
        // Arrange
        var exception = new Exception("Test exception");
        _nextMock.Setup(n => n(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("An exception occurred")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "Should not modify response when no exception occurs")]
    public async Task ShouldNotModifyResponseWhenNoExceptionOccurs()
    {
        // Arrange
        _nextMock.Setup(n => n(It.IsAny<HttpContext>()))
            .Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        _context.Response.StatusCode.Should().Be(200);
        _context.Response.Body.Length.Should().Be(0);
    }

    [Fact(DisplayName = "Should set correct Content-Type header")]
    public async Task ShouldSetCorrectContentTypeHeader()
    {
        // Arrange
        var exception = new Exception("Test exception");
        _nextMock.Setup(n => n(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        _context.Response.ContentType.Should().Be("application/json");
    }

    private async Task<string> GetResponseBody()
    {
        _context.Response.Body.Position = 0;
        using var reader = new StreamReader(_context.Response.Body, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }
}

