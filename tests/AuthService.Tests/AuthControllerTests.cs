namespace AuthService.Tests.Controllers;

using AuthService.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Xunit;

public class AuthControllerTests
{
    private IConfiguration CreateTestConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"Jwt:Key", "TestSecretKeyThatIsLongEnoughForHS256Algorithm12345"},
            {"Jwt:Issuer", "TestAuthService"},
            {"Jwt:Audience", "TestMicroservicesApp"}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [Fact]
    public void Health_ReturnsOkResult()
    {
        // Arrange
        var config = CreateTestConfiguration();
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
        var config = CreateTestConfiguration();
        var controller = new AuthController(config);
        var request = new TokenRequest("testuser");

        // Act
        var result = controller.GenerateToken(request) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().NotBeNull();
        
        // Verify the response structure
        var response = result.Value as TokenResponse;
        response.Should().NotBeNull();
        response!.Token.Should().NotBeNullOrEmpty();
        response.Username.Should().Be("testuser");
        response.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void GenerateToken_WithEmptyUserName_ReturnsBadRequest()
    {
        // Arrange
        var config = CreateTestConfiguration();
        var controller = new AuthController(config);
        var request = new TokenRequest("");

        // Act
        var result = controller.GenerateToken(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void GenerateToken_CreatesValidJwtToken()
    {
        // Arrange
        var config = CreateTestConfiguration();
        var controller = new AuthController(config);
        var request = new TokenRequest("testuser");

        // Act
        var result = controller.GenerateToken(request) as OkObjectResult;
        var response = result!.Value as TokenResponse;

        // Assert
        response!.Token.Should().NotBeNullOrEmpty();
        
        // JWT tokens have 3 parts separated by dots
        var tokenParts = response.Token.Split('.');
        tokenParts.Should().HaveCount(3, "JWT tokens consist of header.payload.signature");
    }
}
