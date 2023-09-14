using BlogAPI.Controllers;
using BlogAPI.Core.Dtos;
using BlogAPI.Core.Entities;
using BlogAPI.Core.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;


namespace BlogAPI.UnitTests.Tests.BlogControllerTests
{
    public class UpdateTests
    {
        private readonly Mock<IBlogService> _mockBlogService;
        private readonly BlogController _controller;
        public UpdateTests()
        {
            _mockBlogService = new Mock<IBlogService>();
            _controller = new BlogController(_mockBlogService.Object);
        }

        [Fact]
        public async Task UpdateBlog_ErrorDuringUpdate_ReturnsInternalServerError()
        {
            // Arrange
            int validBlogId = 1;
            var validBlogUpdateDto = new BlogUpdateDto { Title = "Updated Title", Body = "Updated Body" };

            var mockService = new Mock<IBlogService>();
            mockService.Setup(service => service.UpdateBlogAsync(validBlogId, validBlogUpdateDto))
                       .ThrowsAsync(new Exception("Error during update."));

            var controller = new BlogController(mockService.Object);

            // Act
            var result = await controller.UpdateBlog(validBlogId, validBlogUpdateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task UpdateBlog_InvalidInput_ReturnsBadRequestResult()
        {
            // Arrange
            int validBlogId = 1;
            var invalidBlogUpdateDto = new BlogUpdateDto();

            var mockService = new Mock<IBlogService>();
            mockService.Setup(service => service.UpdateBlogAsync(validBlogId, invalidBlogUpdateDto))
                       .ThrowsAsync(new ArgumentException("Invalid input."));

            var controller = new BlogController(mockService.Object);

            // Act
            var result = await controller.UpdateBlog(validBlogId, invalidBlogUpdateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input.", badRequestResult.Value);
        }
    }
}
