namespace ProductService.Tests.Controllers;

using ProductService.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;


public class ProductControllerTests
{
    [Fact]
    public void Health_ReturnsOkResult()
    {
        // Arrange
        var controller = new ProductController(); 

        // Act
        var result = controller.Health();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void GetAll_ReturnsProductList()
    {
        // Arrange
        var controller = new ProductController();

        // Act
        var result = controller.GetAll() as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().BeAssignableTo<IEnumerable<Product>>();
    }

    [Fact]
    public void GetById_WithValidId_ReturnsProduct()
    {
        // Arrange
        var controller = new ProductController();

        // Act
        var result = controller.GetById(1) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().BeOfType<Product>();
        var product = result.Value as Product;
        product!.Id.Should().Be(1);
    }

    [Fact]
    public void GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var controller = new ProductController();

        // Act
        var result = controller.GetById(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Create_WithValidProduct_ReturnsCreated()
    {
        // Arrange
        var controller = new ProductController();
        var request = new CreateProductRequest("New Product", 49.99m, true);

        // Act
        var result = controller.Create(request) as CreatedAtActionResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().BeOfType<Product>();
        var product = result.Value as Product;
        product!.Name.Should().Be("New Product");
        product.Price.Should().Be(49.99m);
    }

    [Fact]
    public void Create_WithEmptyName_ReturnsBadRequest()
    {
        // Arrange
        var controller = new ProductController();
        var request = new CreateProductRequest("", 49.99m, true);

        // Act
        var result = controller.Create(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Create_WithNegativePrice_ReturnsBadRequest()
    {
        // Arrange
        var controller = new ProductController();
        var request = new CreateProductRequest("Product", -10m, true);

        // Act
        var result = controller.Create(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Update_WithValidData_ReturnsOk()
    {
        // Arrange
        var controller = new ProductController();
        var request = new UpdateProductRequest("Updated Name", 99.99m, false);

        // Act
        var result = controller.Update(1, request) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().BeOfType<Product>();
        var product = result.Value as Product;
        product!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public void Update_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var controller = new ProductController();
        var request = new UpdateProductRequest("Updated", 99.99m, true);

        // Act
        var result = controller.Update(999, request);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Delete_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var controller = new ProductController();

        // Act
        var result = controller.Delete(2);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void Delete_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var controller = new ProductController();

        // Act
        var result = controller.Delete(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
