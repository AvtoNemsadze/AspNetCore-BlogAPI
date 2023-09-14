using BlogAPI.Controllers;
using BlogAPI.Core.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;


namespace BlogAPI.UnitTests.Tests.BlogControllerTests
{
    public class DeleteTests
    {
        private readonly Mock<IBlogService> _mockBlogService;
        private readonly BlogController _controller;
        public DeleteTests()
        {
            _mockBlogService = new Mock<IBlogService>();
            _controller = new BlogController(_mockBlogService.Object);
        }

        [Fact]
        public async Task DeleteBlog_ValidId_ReturnsOkResult()
        {
            // Arrange
            int validBlogId = 1;
            var mockService = new Mock<IBlogService>();

            var controller = new BlogController(mockService.Object);

            // Act
            var result = await controller.DeleteBlog(validBlogId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Blog deleted successfully", okResult.Value);
        }

        [Fact]
        public async Task DeleteBlog_BlogNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            int nonExistentBlogId = 999; 
            var mockService = new Mock<IBlogService>();
            mockService.Setup(service => service.DeleteBlogAsync(nonExistentBlogId))
                       .ThrowsAsync(new NullReferenceException("Blog Not Found"));

            var controller = new BlogController(mockService.Object);

            // Act
            var result = await controller.DeleteBlog(nonExistentBlogId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Blog not found", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteBlog_DbUpdateException_ReturnsBadRequestResult()
        {
            // Arrange
            int validBlogId = 1;
            var mockService = new Mock<IBlogService>();
            mockService.Setup(service => service.DeleteBlogAsync(validBlogId))
                       .ThrowsAsync(new DbUpdateException("Error during deletion"));

            var controller = new BlogController(mockService.Object);

            // Act
            var result = await controller.DeleteBlog(validBlogId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("An error occurred while deleting the blog post. Please check related constraints.", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteBlog_GenericException_ReturnsInternalServerError()
        {
            // Arrange
            int validBlogId = 1;
            var mockService = new Mock<IBlogService>();
            mockService.Setup(service => service.DeleteBlogAsync(validBlogId))
                       .ThrowsAsync(new Exception("Generic error"));

            var controller = new BlogController(mockService.Object);

            // Act
            var result = await controller.DeleteBlog(validBlogId);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerErrorResult.StatusCode);
            Assert.Equal("An error occurred while deleting the blog post.", internalServerErrorResult.Value);
        }
    }
}
