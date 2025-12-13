namespace AuthService.Tests.Controllers;

using AuthService.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Xunit;
public class AuthControllerTests
{
    [Fact]
    public void Health_ReturnsOkResult()
    {
        // Arrange
        var config = new ConfigurationBuilder().Build();
        var controller = new AuthController(config);

        // Act
        var result = controller.Health();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void GenerateToken_WithValidUsername_ReturnsToken()
    {
        // Arrange
        var config = new ConfigurationBuilder().Build();
        var controller = new AuthController(config);
        var request = new TokenRequest("testuser");

        // Act
        var result = controller.GenerateToken(request) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().NotBeNull();
    }

    [Fact]
    public void GenerateToken_WithEmptyUserName_ReturnsBadRequest()
    {
        // Arrange
        var config = new ConfigurationBuilder().Build();
        var controller = new AuthController(config);
        var request = new TokenRequest("");

        // Act
        var result = controller.GenerateToken(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
