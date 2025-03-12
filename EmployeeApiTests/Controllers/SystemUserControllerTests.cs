using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserApi.Common;
using UserApi.Controllers;
using UserApi.Repository.DTOs;
using UserApi.Services;

namespace EmployeeApiTests.Controllers
{
    public class SystemUserControllerTests
    {
        private readonly Mock<ISystemUserService> _userServiceMock;
        private readonly SystemUserController _controller;

        public SystemUserControllerTests()
        {
            _userServiceMock = new Mock<ISystemUserService>();
            _controller = new SystemUserController(_userServiceMock.Object);
        }

        #region GET Endpoints

        [Fact]
        public void GetAllUsers_ReturnsOk_WithUsersList()
        {
            // Arrange
            var users = new List<SystemUserDto>
            {
                new SystemUserDto { UserId = 1, Email = "user1@example.com" },
                new SystemUserDto { UserId = 2, Email = "user2@example.com" }
            };
            _userServiceMock.Setup(s => s.GetUsers()).Returns(users);

            // Act
            var result = _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(users, okResult.Value);
        }

        [Fact]
        public void GetActiveUsers_ReturnsOk_WithActiveUsersList()
        {
            // Arrange
            var activeUsers = new List<SystemUserDto>
            {
                new SystemUserDto { UserId = 3, Email = "active@example.com" }
            };
            _userServiceMock.Setup(s => s.GetActiveUsers()).Returns(activeUsers);

            // Act
            var result = _controller.GetActiveUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(activeUsers, okResult.Value);
        }

        #endregion

        #region Soft Delete Endpoints (PATCH)

        [Fact]
        public void SoftDeleteUserById_ReturnsNotFound_WhenServiceReturns404()
        {
            // Arrange
            var opResult = OperationResult.Fail("User not found", StatusCodes.Status404NotFound);
            _userServiceMock.Setup(s => s.SoftDeleteUser(It.IsAny<int>())).Returns(opResult);

            // Act
            var result = _controller.SoftDeleteUserById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            // Use reflection to access anonymous object's Message property.
            var messageProperty = notFoundResult.Value.GetType().GetProperty("Message");
            Assert.NotNull(messageProperty);
            var message = messageProperty.GetValue(notFoundResult.Value) as string;
            Assert.Equal("User not found", message);
        }

        [Fact]
        public void SoftDeleteUserById_ReturnsProblem_WhenServiceReturnsErrors()
        {
            // Arrange: simulate an operation result with errors (non-empty Errors list).
            var opResult = OperationResult.Fail("Unexpected error", StatusCodes.Status500InternalServerError);
            _userServiceMock.Setup(s => s.SoftDeleteUser(It.IsAny<int>())).Returns(opResult);

            // Act
            var result = _controller.SoftDeleteUserById(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        [Fact]
        public void SoftDeleteUserById_ReturnsNoContent_WhenServiceSucceeds()
        {
            // Arrange: simulate a successful operation result.
            var opResult = OperationResult.Ok();
            _userServiceMock.Setup(s => s.SoftDeleteUser(It.IsAny<int>())).Returns(opResult);

            // Act
            var result = _controller.SoftDeleteUserById(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void SoftDeleteUserByMail_ReturnsNotFound_WhenServiceReturns404()
        {
            // Arrange
            var opResult = OperationResult.Fail("User not found", StatusCodes.Status404NotFound);
            _userServiceMock.Setup(s => s.SoftDeleteUser(It.IsAny<string>())).Returns(opResult);

            // Act
            var result = _controller.SoftDeleteUserById("test@example.com");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var messageProperty = notFoundResult.Value.GetType().GetProperty("Message");
            Assert.NotNull(messageProperty);
            var message = messageProperty.GetValue(notFoundResult.Value) as string;
            Assert.Equal("User not found", message);
        }

        [Fact]
        public void SoftDeleteUserByMail_ReturnsNoContent_WhenServiceSucceeds()
        {
            // Arrange
            var opResult = OperationResult.Ok();
            _userServiceMock.Setup(s => s.SoftDeleteUser(It.IsAny<string>())).Returns(opResult);

            // Act
            var result = _controller.SoftDeleteUserById("test@example.com");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        #endregion

        #region Hard Delete Endpoint (DELETE)

        [Fact]
        public void HardDeleteUserById_ReturnsNotFound_WhenServiceReturns404()
        {
            // Arrange
            var opResult = OperationResult.Fail("User not found", StatusCodes.Status404NotFound);
            _userServiceMock.Setup(s => s.HardDeleteUser(It.IsAny<int>())).Returns(opResult);

            // Act
            var result = _controller.HardDeleteUserById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var messageProperty = notFoundResult.Value.GetType().GetProperty("Message");
            Assert.NotNull(messageProperty);
            var message = messageProperty.GetValue(notFoundResult.Value) as string;
            Assert.Equal("User not found", message);
        }

        [Fact]
        public void HardDeleteUserById_ReturnsNoContent_WhenServiceSucceeds()
        {
            // Arrange
            var opResult = OperationResult.Ok();
            _userServiceMock.Setup(s => s.HardDeleteUser(It.IsAny<int>())).Returns(opResult);

            // Act
            var result = _controller.HardDeleteUserById(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        #endregion
    }
}
