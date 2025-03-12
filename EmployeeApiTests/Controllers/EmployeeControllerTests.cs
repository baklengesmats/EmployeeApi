using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserApi.Common;
using UserApi.Controllers;
using UserApi.Repository.DTOs;
using UserApi.Services;

namespace EmployeeApiTests.Controllers
{
    public class EmployeeControllerTests
    {
        private readonly Mock<IEmployeeService> _serviceMock;
        private readonly EmployeeController _controller;

        public EmployeeControllerTests()
        {
            _serviceMock = new Mock<IEmployeeService>();
            _controller = new EmployeeController(_serviceMock.Object);
        }

        #region AddEmployees Tests

        [Fact]
        public void AddEmployees_ReturnsProblem_WhenServiceReturnsError()
        {
            // Arrange
            var opResultError = OperationResult.Fail("Error occurred", 400);
            _serviceMock.Setup(s => s.AddEmployee("mail@example.com", "John", "Doe"))
                        .Returns(opResultError);

            // Act
            var result = _controller.AddEmployees("John", "Doe", "mail@example.com");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public void AddEmployees_ReturnsCreated_WhenServiceReturnsSuccess()
        {
            // Arrange
            var opResultSuccess = OperationResult.Ok();
            _serviceMock.Setup(s => s.AddEmployee("mail@example.com", "John", "Doe"))
                        .Returns(opResultSuccess);

            // Act
            var result = _controller.AddEmployees("John", "Doe", "mail@example.com");

            // Assert: Created() returns a CreatedResult.
            Assert.IsType<CreatedResult>(result);
        }

        #endregion

        #region GetEmployees Tests

        [Fact]
        public void GetAllEmployees_ReturnsOk_WithEmployeeList()
        {
            // Arrange
            var dtos = new List<EmployeeDto>
            {
                new EmployeeDto { EmployeeId = 1, Email = "a@example.com" },
                new EmployeeDto { EmployeeId = 2, Email = "b@example.com" }
            };
            _serviceMock.Setup(s => s.GetEmployees()).Returns(dtos);

            // Act
            var result = _controller.GetAllEmployees();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dtos, okResult.Value);
        }

        [Fact]
        public void GetActiveEmployees_ReturnsOk_WithActiveEmployeeList()
        {
            // Arrange
            var activeDtos = new List<EmployeeDto>
            {
                new EmployeeDto { EmployeeId = 3, Email = "c@example.com" }
            };
            _serviceMock.Setup(s => s.GetActiveEmployees()).Returns(activeDtos);

            // Act
            var result = _controller.GetActiveEmployees();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(activeDtos, okResult.Value);
        }

        #endregion

        #region ReactiveEmployee Tests

        [Fact]
        public void ReactiveEmployeeById_ReturnsNotFound_WhenServiceFails()
        {
            // Arrange: simulate service returning failure OperationResult (404).
            var opFail = OperationResult.Fail("Couldn't find employee with mail: mail@example.com.", 404);
            _serviceMock.Setup(s => s.ReactiveEmployee(1)).Returns(opFail);

            // Act
            var result = _controller.ReactiveEmployeeById(1);

            // Assert: ProcessOperationResult returns NotFoundObjectResult.
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var messageProperty = notFoundResult.Value.GetType().GetProperty("Message");
            var messageValue = messageProperty?.GetValue(notFoundResult.Value, null) as string;
            Assert.Equal("Couldn't find employee with mail: mail@example.com.", messageValue);
        }

        [Fact]
        public void ReactiveEmployeeById_ReturnsNoContent_WhenServiceSucceeds()
        {
            // Arrange: simulate service returning success.
            var opSuccess = OperationResult.Ok();
            _serviceMock.Setup(s => s.ReactiveEmployee(1)).Returns(opSuccess);

            // Act
            var result = _controller.ReactiveEmployeeById(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void ReactiveEmployeeByMail_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            var opFail = OperationResult.Fail("Couldn't find employee with mail: mail@example.com.", 404);
            _serviceMock.Setup(s => s.ReactiveEmployee("mail@example.com")).Returns(opFail);

            // Act
            var result = _controller.ReactiveEmployeeByMail("mail@example.com");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var messageProperty = notFoundResult.Value.GetType().GetProperty("Message");
            var messageValue = messageProperty?.GetValue(notFoundResult.Value, null) as string;
            Assert.Equal("Couldn't find employee with mail: mail@example.com.", messageValue);
        }

        [Fact]
        public void ReactiveEmployeeByMail_ReturnsNoContent_WhenServiceSucceeds()
        {
            // Arrange
            var opSuccess = OperationResult.Ok();
            _serviceMock.Setup(s => s.ReactiveEmployee("mail@example.com")).Returns(opSuccess);

            // Act
            var result = _controller.ReactiveEmployeeByMail("mail@example.com");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        #endregion

        #region SoftDeleteEmployee Tests

        [Fact]
        public void SoftDeleteEmployeeById_ReturnsNotFound_WhenUserNotFound()
        {
            var opFail = OperationResult.Fail("Couldn't find employee with id: 1.", 404);
            _serviceMock.Setup(s => s.DeactiveEmployee(1)).Returns(opFail);
            // Act
            var result = _controller.SoftDeleteEmployeeById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var messageProperty = notFoundResult.Value.GetType().GetProperty("Message");
            var messageValue = messageProperty?.GetValue(notFoundResult.Value, null) as string;
            Assert.Equal("Couldn't find employee with id: 1.", messageValue);
        }

        [Fact]
        public void SoftDeleteEmployeeById_ReturnsNotFound_WhenServiceFails()
        {
            var opFail = OperationResult.Fail("An unexpected error occurred.", 500);
            _serviceMock.Setup(s => s.DeactiveEmployee(1)).Returns(opFail);
            // Act
            var result = _controller.SoftDeleteEmployeeById(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal("An unexpected error occurred.", problemDetails.Title);
        }

        [Fact]
        public void SoftDeleteEmployeeById_ReturnsNoContent_WhenServiceSucceeds()
        {
            // Arrange
            var opSuccess = OperationResult.Ok();
            _serviceMock.Setup(s => s.DeactiveEmployee(1)).Returns(opSuccess);

            // Act
            var result = _controller.SoftDeleteEmployeeById(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void SoftDeleteEmployeeByMail_ReturnsNotFound_WhenServiceFails()
        {
            // Arrange
            var opFail = OperationResult.Fail("User not found", 404);
            _serviceMock.Setup(s => s.DeactiveEmployee("mail@example.com")).Returns(opFail);

            // Act
            var result = _controller.SoftDeleteEmployeeByMail("mail@example.com");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var messageProperty = notFoundResult.Value.GetType().GetProperty("Message");
            var messageValue = messageProperty?.GetValue(notFoundResult.Value, null) as string;
            Assert.Equal("User not found", messageValue);
        }

        [Fact]
        public void SoftDeleteEmployeeByMail_ReturnsNoContent_WhenServiceSucceeds()
        {
            // Arrange
            var opSuccess = OperationResult.Ok();
            _serviceMock.Setup(s => s.DeactiveEmployee("mail@example.com")).Returns(opSuccess);

            // Act
            var result = _controller.SoftDeleteEmployeeByMail("mail@example.com");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        #endregion

        #region HardDeleteEmployee Tests

        [Fact]
        public void HardDeleteEmployeeById_ReturnsNotFound_WhenServiceFails()
        {
            // Arrange
            var opFail = OperationResult.Fail("User not found", 404);
            _serviceMock.Setup(s => s.DeleteEmployee(1)).Returns(opFail);

            // Act
            var result = _controller.HardDeleteEmployeeById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var messageProperty = notFoundResult.Value.GetType().GetProperty("Message");
            var messageValue = messageProperty?.GetValue(notFoundResult.Value, null) as string;
            Assert.Equal("User not found", messageValue);
        }

        [Fact]
        public void HardDeleteEmployeeById_ReturnsNoContent_WhenServiceSucceeds()
        {
            // Arrange
            var opSuccess = OperationResult.Ok();
            _serviceMock.Setup(s => s.DeleteEmployee(1)).Returns(opSuccess);

            // Act
            var result = _controller.HardDeleteEmployeeById(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        #endregion
    }
}

