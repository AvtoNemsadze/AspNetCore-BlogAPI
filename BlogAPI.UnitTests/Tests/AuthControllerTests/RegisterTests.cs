using BlogAPI.Controllers;
using BlogAPI.Core.Common;
using BlogAPI.Core.Dtos;
using BlogAPI.Core.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;


namespace BlogAPI.UnitTests.Tests.AuthControllerTests
{
    public class RegisterTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;
        public RegisterTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public async Task Register_ValidData_ReturnsOkResult()
        {
            // Arrange
            var validRegisterDto = new RegisterDto
            {
                FirstName = "Avto",
                LastName = "Nemsadze",
                Email = "Avto.Nemsadze@example.com",
                UserName = "avtoNemsadze",
                Password = "Password12345"
            };

            var mockService = new Mock<IAuthService>();
            mockService.Setup(service => service.RegisterAsync(validRegisterDto))
                       .ReturnsAsync(new AuthServiceResponse { IsSucceed = true, Message = "User Created Successfully" });

            var controller = new AuthController(mockService.Object);

            // Act
            var result = await controller.Register(validRegisterDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var authServiceResponse = Assert.IsAssignableFrom<AuthServiceResponse>(okResult.Value);
            Assert.True(authServiceResponse.IsSucceed);
            Assert.Equal("User Created Successfully", authServiceResponse.Message);
        }

        [Fact]
        public async Task Register_DuplicateUsername_ReturnsBadRequest()
        {
            // Arrange
            var duplicateUsernameDto = new RegisterDto
            {
                FirstName = "Avto",
                LastName = "Nemsadze",
                Email = "Avto.Nemsadze@example.com",
                UserName = "avtoNemsadze",
                Password = "Password12345"
            };

            var mockService = new Mock<IAuthService>();
            mockService.Setup(service => service.RegisterAsync(duplicateUsernameDto))
                       .ReturnsAsync(new AuthServiceResponse { IsSucceed = false, Message = "UserName Already Exsist" });

            var controller = new AuthController(mockService.Object);

            // Act
            var result = await controller.Register(duplicateUsernameDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var authServiceResponse = Assert.IsAssignableFrom<AuthServiceResponse>(badRequestResult.Value);
            Assert.False(authServiceResponse.IsSucceed);
            Assert.Equal("UserName Already Exsist", authServiceResponse.Message);
        }

        
        [Fact]
        public async Task Register_InvalidData_ReturnsBadRequestWithModelStateErrors()
        {
            // Arrange
            var invalidRegisterDto = new RegisterDto();
            var mockService = new Mock<IAuthService>();
            var controller = new AuthController(mockService.Object);
            controller.ModelState.AddModelError("UserName", "UserName is required");

            // Act
            var result = await controller.Register(invalidRegisterDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);

            Assert.Contains("UserName is required", modelState["UserName"] as string[]);
        }
    }
}
