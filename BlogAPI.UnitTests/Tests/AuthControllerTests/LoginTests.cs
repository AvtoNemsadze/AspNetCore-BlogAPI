using BlogAPI.Controllers;
using BlogAPI.Core.Common;
using BlogAPI.Core.Dtos;
using BlogAPI.Core.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;


namespace BlogAPI.UnitTests.Tests.AuthControllerTests
{
    public class LoginTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        public LoginTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var validLoginDto = new LoginDto
            {
                UserName = "testuser",
                Password = "Password123"
            };

            var authServiceMock = new Mock<IAuthService>();
            authServiceMock.Setup(service => service.LoginAsync(validLoginDto))
                .ReturnsAsync(new AuthServiceResponse
                {
                    IsSucceed = true,
                    Message = "your-token-here"
                });

            var controller = new AuthController(authServiceMock.Object);

            // Act
            var result = await controller.Login(validLoginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var authServiceResponse = Assert.IsAssignableFrom<AuthServiceResponse>(okResult.Value);

            Assert.True(authServiceResponse.IsSucceed);
            Assert.Equal("your-token-here", authServiceResponse.Message);
        }

    }
}
