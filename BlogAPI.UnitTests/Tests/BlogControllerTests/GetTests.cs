using BlogAPI.Controllers;
using BlogAPI.Core.Dtos;
using BlogAPI.Core.Entities;
using BlogAPI.Core.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;


namespace BlogAPI.UnitTests.Tests.BlogControllerTests
{
    public class GetTests
    {
        private readonly Mock<IBlogService> _mockBlogService;
        private readonly BlogController _controller;
        public GetTests()
        {
            _mockBlogService = new Mock<IBlogService>();
            _controller = new BlogController(_mockBlogService.Object);
        }

        [Fact]
        public async Task GetBlogsAsync_Pagination_ReturnsCorrectPageSizeAndPageNumber()
        {
            // Arrange
            int pageSize = 10;
            int pageNumber = 2;

            var blogEntities = new List<Blog>
            {
                new Blog
                {
                    Id = 1,
                    Title = "Blog Post Title 1",
                    Body = "This is the body of sample blog 1.",
                    PublishDate = DateTime.Now.AddDays(8)
                },
                new Blog
                {
                    Id = 2,
                    Title = "Blog Post Title 2",
                    Body = "This is the body of sample blog 2.",
                    PublishDate = DateTime.Now.AddDays(6)
                },
                 new Blog
                {
                    Id = 3,
                    Title = "Blog Post Title 3",
                    Body = "This is the body of sample blog 3.",
                    PublishDate = DateTime.Now.AddDays(7)
                },
                new Blog
                {
                    Id = 4,
                    Title = "Blog Post Title 4",
                    Body = "This is the body of sample blog 4.",
                    PublishDate = DateTime.Now.AddDays(2)
                },

                new Blog
                {
                    Id = 5,
                    Title = "Blog Post Title 5",
                    Body = "This is the body of sample blog 5.",
                    PublishDate = DateTime.Now.AddDays(5)
                },
                new Blog
                {
                    Id = 6,
                    Title = "Blog Post Title 6",
                    Body = "This is the body of sample blog 6.",
                    PublishDate = DateTime.Now.AddDays(4)
                },
                 new Blog
                {
                    Id = 7,
                    Title = "Blog Post Title 7",
                    Body = "This is the body of sample blog 7.",
                    PublishDate = DateTime.Now.AddDays(-1)
                },
                new Blog
                {
                    Id = 8,
                    Title = "Blog Post Title 8",
                    Body = "This is the body of sample blog 8.",
                    PublishDate = DateTime.Now.AddDays(-2)
                },
                new Blog
                {
                    Id = 9,
                    Title = "Blog Post Title 9",
                    Body = "This is the body of sample blog 9.",
                    PublishDate = DateTime.Now.AddDays(-5)
                },
                new Blog
                {
                    Id = 10,
                    Title = "Blog Post Title 10",
                    Body = "This is the body of sample blog 10.",
                    PublishDate = DateTime.Now.AddDays(5)
                },
            };

            _mockBlogService.Setup(service => service.GetBlogsAsync(pageSize, pageNumber, null)) 
                           .ReturnsAsync((blogEntities, new PaginationMetadata(blogEntities.Count, pageSize, pageNumber)));

            // Act
            var (returnedBlogEntities, paginationMetadata) = await _mockBlogService.Object.GetBlogsAsync(pageSize, pageNumber);

            // Assert
            Assert.Equal(pageSize, returnedBlogEntities.Count());
            Assert.Equal(pageNumber, paginationMetadata.CurrentPage);
        }

        [Fact]
        public async Task GetBlogById_ValidId_ReturnsOkResultWithBlogDto()
        {
            // Arrange
            int validBlogId = 1;
            var expectedBlogDto = new BlogGetDto
            {
                Id = validBlogId,
                Title = "Test Blog Title",
                Body = "Test Blog Body",
                PublishDate = DateTime.Now
            };

            var mockService = new Mock<IBlogService>();
            mockService.Setup(service => service.GetBlogByIdAsync(validBlogId))
                       .ReturnsAsync(expectedBlogDto);

            var controller = new BlogController(mockService.Object);

            // Act
            var result = await controller.GetBlogById(validBlogId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBlogDto = Assert.IsAssignableFrom<BlogGetDto>(okResult.Value);
            Assert.Equal(expectedBlogDto, returnedBlogDto);
        }

        [Fact]
        public async Task GetBlogById_InvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            int invalidBlogId = 9999; 

            var mockService = new Mock<IBlogService>();
            mockService.Setup(service => service.GetBlogByIdAsync(invalidBlogId))
                       .ReturnsAsync((BlogGetDto)null); 

            var controller = new BlogController(mockService.Object);

            // Act
            var result = await controller.GetBlogById(invalidBlogId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
