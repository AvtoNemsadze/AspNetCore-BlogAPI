using Moq;
using BlogAPI.Controllers;
using BlogAPI.Core.Interface;
using BlogAPI.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using BlogAPI.Core.Entities;

namespace BlogAPI.UnitTests.Controllers
{
    public class BlogControllerTests
    {
        private readonly Mock<IBlogService> _mockBlogService;
        private readonly BlogController _controller;
        public BlogControllerTests()
        {
            _mockBlogService = new Mock<IBlogService>();
            _controller = new BlogController(_mockBlogService.Object);
        }

        //[Fact]
        //public async Task CreateBlog_ValidInput_ReturnsOkResult()
        //{
        //    // Arrange
        //    var validBlogCreateDto = new BlogCreateDto { Title = "Test Title", Body = "Test Body" };
        //    _mockBlogService.Setup(service => service.CreateBlogAsync(validBlogCreateDto)).ReturnsAsync(new Blog());

        //    // Act
        //    var result = await _controller.CreateBlog(validBlogCreateDto);

        //    // Assert
        //    var okResult = Assert.IsType<OkResult>(result);
        //    Assert.Equal(200, okResult.StatusCode);
        //}

        //[Fact]
        //public async Task CreateBlog_InvalidInput_ReturnsBadRequest()
        //{
        //    // Arrange
        //    var invalidBlogCreateDto = new BlogCreateDto(); // This should be invalid.
        //    _controller.ModelState.AddModelError("Title", "Title is required");

        //    // Act
        //    var result = await _controller.CreateBlog(invalidBlogCreateDto);

        //    // Assert
        //    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        //    Assert.Equal(400, badRequestResult.StatusCode);
        //    Assert.Equal("The blogCreateDto parameter is null.", badRequestResult.Value);
        //}

    }
}
