using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserApi.Controllers;
using UserApi.Services;

namespace EmployeeApiTests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public void Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            string email = "test@example.com";
            string password = "wrongPassword";

            // Setup the auth service to return false when validating the login.
            _authServiceMock.Setup(s => s.ValidateLogin(email, password))
                            .Returns(false);

            // Act
            var result = _controller.Login(email, password);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public void Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            string email = "test@example.com";
            string password = "correctPassword";

            // Setup the auth service for a valid login.
            _authServiceMock.Setup(s => s.ValidateLogin(email, password))
                            .Returns(true);
            _authServiceMock.Setup(s => s.GenerateJwtToken(email))
                            .Returns("fake-jwt-token");

            // Act
            var result = _controller.Login(email, password);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var tokenProperty = okResult.Value.GetType().GetProperty("Token");
            Assert.NotNull(tokenProperty);
            var tokenValue = tokenProperty.GetValue(okResult.Value) as string;
            Assert.Equal("fake-jwt-token", tokenValue);
        }
    }
}

