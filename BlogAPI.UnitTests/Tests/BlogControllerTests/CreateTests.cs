using BlogAPI.Controllers;
using BlogAPI.Core.Dtos;
using BlogAPI.Core.Entities;
using BlogAPI.Core.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BlogAPI.UnitTests.Tests.BlogControllerTests
{
    public class CreateTests
    {
        private readonly Mock<IBlogService> _mockBlogService;
        private readonly BlogController _controller;
        public CreateTests()
        {
            _mockBlogService = new Mock<IBlogService>();
            _controller = new BlogController(_mockBlogService.Object);
        }

        [Fact]
        public async Task CreateBlog_ValidInput_ReturnsOkObjectResult()
        {
            // Arrange
            var validBlogCreateDto = new BlogCreateDto { Title = "Test Title", Body = "Test Body" };
            _mockBlogService.Setup(service => service.CreateBlogAsync(validBlogCreateDto)).ReturnsAsync(new Blog());

            // Act
            var result = await _controller.CreateBlog(validBlogCreateDto);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }

        [Fact]
        public async Task CreateBlog_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var invalidBlogCreateDto = new BlogCreateDto(); // This should be invalid.
            _controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _controller.CreateBlog(invalidBlogCreateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);

            var errors = (SerializableError)badRequestResult.Value;
            Assert.True(errors.ContainsKey("Title"));
            var errorMessages = (string[])errors["Title"];
            Assert.Contains("Title is required", errorMessages);
        }

        
        [Fact]
        public async Task CreateBlog_ValidData_ReturnsOkResult()
        {
            // Arrange
            var blogCreateDto = new BlogCreateDto { Title = "Test Title", Body = "Test Body" };

            // Act
            var result = await _controller.CreateBlog(blogCreateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Blog Created Successfully", okResult.Value);
        }

    }
}
