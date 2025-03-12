using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using UserApi.Repository.DTOs;
using UserApi.Repository.Model;
using UserApi.Repository;
using UserApi.Services;

namespace EmployeeApiTests.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            _repoMock = new Mock<IEmployeeRepository>();
            _mapperMock = new Mock<IMapper>();
            _employeeService = new EmployeeService(_repoMock.Object, _mapperMock.Object);
        }

        #region AddEmployee Tests

        [Fact]
        public void AddEmployee_MissingEmail_ReturnsFail()
        {
            // Arrange
            string email = "";
            string firstName = "John";
            string lastName = "Doe";

            // Act
            var result = _employeeService.AddEmployee(email, firstName, lastName);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public void AddEmployee_MissingFirstName_ReturnsFail()
        {
            // Arrange
            string email = "john@example.com";
            string firstName = "";
            string lastName = "Doe";

            // Act
            var result = _employeeService.AddEmployee(email, firstName, lastName);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public void AddEmployee_MissingLastName_ReturnsFail()
        {
            // Arrange
            string email = "john@example.com";
            string firstName = "John";
            string lastName = "";

            // Act
            var result = _employeeService.AddEmployee(email, firstName, lastName);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public void AddEmployee_DuplicateEmail_ReturnsFail()
        {
            // Arrange
            string email = "john@example.com";
            string firstName = "John";
            string lastName = "Doe";

            // Setup repository to simulate a duplicate email
            _repoMock.Setup(r => r.GetEmployee(email)).Returns(new Employee { Email = email });

            // Act
            var result = _employeeService.AddEmployee(email, firstName, lastName);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(409, result.StatusCode);
        }

        [Fact]
        public void AddEmployee_ValidInput_ReturnsOk()
        {
            // Arrange
            string email = "john@example.com";
            string firstName = "John";
            string lastName = "Doe";

            // Ensure no duplicate exists
            _repoMock.Setup(r => r.GetEmployee(email)).Returns((Employee?)null);

            // Act
            var result = _employeeService.AddEmployee(email, firstName, lastName);

            // Assert
            Assert.True(result.Success);
            _repoMock.Verify(r => r.Add(It.Is<Employee>(e =>
                e.Email == email &&
                e.FirstName == firstName &&
                e.LastName == lastName
            )), Times.Once);
        }

        #endregion

        #region DeactiveEmployee Tests

        [Fact]
        public void DeactiveEmployee_ById_EmployeeNotFound_ReturnsFail()
        {
            // Arrange
            int employeeId = 1;
            _repoMock.Setup(r => r.GetEmployee(employeeId)).Returns((Employee?)null);

            // Act
            var result = _employeeService.DeactiveEmployee(employeeId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public void DeactiveEmployee_ById_EmployeeFound_ReturnsOk()
        {
            // Arrange
            int employeeId = 1;
            var employee = new Employee { EmployeeId = employeeId, Email = "john@example.com", Created = DateTime.Now };
            _repoMock.Setup(r => r.GetEmployee(employeeId)).Returns(employee);

            // Act
            var result = _employeeService.DeactiveEmployee(employeeId);

            // Assert
            Assert.True(result.Success);
            _repoMock.Verify(r => r.SoftDelete(employeeId), Times.Once);
        }

        [Fact]
        public void DeactiveEmployee_ByEmail_EmployeeNotFound_ReturnsFail()
        {
            // Arrange
            string email = "john@example.com";
            _repoMock.Setup(r => r.GetEmployee(email)).Returns((Employee?)null);

            // Act
            var result = _employeeService.DeactiveEmployee(email);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public void DeactiveEmployee_ByEmail_EmployeeFound_ReturnsOk()
        {
            // Arrange
            string email = "john@example.com";
            var employee = new Employee { EmployeeId = 1, Email = email, Created = DateTime.Now };
            _repoMock.Setup(r => r.GetEmployee(email)).Returns(employee);

            // Act
            var result = _employeeService.DeactiveEmployee(email);

            // Assert
            Assert.True(result.Success);
            _repoMock.Verify(r => r.SoftDelete(email), Times.Once);
        }

        #endregion

        #region DeleteEmployee Tests

        [Fact]
        public void DeleteEmployee_EmployeeNotFound_ReturnsFail()
        {
            // Arrange
            int employeeId = 1;
            _repoMock.Setup(r => r.GetEmployee(employeeId)).Returns((Employee?)null);

            // Act
            var result = _employeeService.DeleteEmployee(employeeId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
        }


        [Fact]
        public void DeleteEmployee_EmployeeFound_ReturnsOk()
        {
            // Arrange
            int employeeId = 1;
            var employee = new Employee { EmployeeId = employeeId, Email = "john@example.com", Created = DateTime.Now };
            _repoMock.Setup(r => r.GetEmployee(employeeId)).Returns(employee);

            // Act
            var result = _employeeService.DeleteEmployee(employeeId);

            // Assert
            Assert.True(result.Success);
            _repoMock.Verify(r => r.HardDeleteById(employeeId), Times.Once);
        }

        #endregion

        #region ReactiveEmployee Tests

        [Fact]
        public void ReactiveEmployee_ById_EmployeeNotFound_ReturnsFail()
        {
            // Arrange
            int employeeId = 1;
            _repoMock.Setup(r => r.GetEmployee(employeeId)).Returns((Employee?)null);

            // Act
            var result = _employeeService.ReactiveEmployee(employeeId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public void ReactiveEmployee_ById_EmployeeFound_ReturnsOk()
        {
            // Arrange
            int employeeId = 1;
            var employee = new Employee
            {
                EmployeeId = employeeId,
                Email = "john@example.com",
                Created = DateTime.Now,
                Deleted = DateTime.Now
            };
            _repoMock.Setup(r => r.GetEmployee(employeeId)).Returns(employee);

            // Act
            var result = _employeeService.ReactiveEmployee(employeeId);

            // Assert
            Assert.True(result.Success);
            _repoMock.Verify(r => r.Reactive(employeeId), Times.Once);
        }

        [Fact]
        public void ReactiveEmployee_ByEmail_EmployeeNotFound_ReturnsFail()
        {
            // Arrange
            string email = "john@example.com";
            _repoMock.Setup(r => r.GetEmployee(email)).Returns((Employee?)null);

            // Act
            var result = _employeeService.ReactiveEmployee(email);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public void ReactiveEmployee_ByEmail_EmployeeFound_ReturnsOk()
        {
            // Arrange
            string email = "john@example.com";
            var employee = new Employee
            {
                EmployeeId = 1,
                Email = email,
                Created = DateTime.Now,
                Deleted = DateTime.Now
            };
            _repoMock.Setup(r => r.GetEmployee(email)).Returns(employee);

            // Act
            var result = _employeeService.ReactiveEmployee(email);

            // Assert
            Assert.True(result.Success);
            _repoMock.Verify(r => r.Reactive(email), Times.Once);
        }

        #endregion

        #region Get Methods Tests

        [Fact]
        public void GetActiveEmployees_ReturnsMappedEmployeeDtos()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { EmployeeId = 1, Email = "active1@example.com", Created = DateTime.Now, Deleted = null },
                new Employee { EmployeeId = 2, Email = "active2@example.com", Created = DateTime.Now, Deleted = null }
            };
            _repoMock.Setup(r => r.GetActiveEmployees()).Returns(employees);

            var expectedDtos = new List<EmployeeDto>
            {
                new EmployeeDto { EmployeeId = 1, Email = "active1@example.com" },
                new EmployeeDto { EmployeeId = 2, Email = "active2@example.com" }
            };
            _mapperMock.Setup(m => m.Map<List<EmployeeDto>>(employees)).Returns(expectedDtos);

            // Act
            var result = _employeeService.GetActiveEmployees();

            // Assert
            Assert.Equal(expectedDtos.Count, result.Count);
            Assert.Equal(expectedDtos[0].Email, result[0].Email);
        }

        [Fact]
        public void GetEmployee_ById_ReturnsMappedEmployeeDto()
        {
            // Arrange
            int employeeId = 1;
            var employee = new Employee { EmployeeId = employeeId, Email = "john@example.com", Created = DateTime.Now };
            _repoMock.Setup(r => r.GetEmployee(employeeId)).Returns(employee);

            var expectedDto = new EmployeeDto { EmployeeId = employeeId, Email = "john@example.com" };
            _mapperMock.Setup(m => m.Map<EmployeeDto>(employee)).Returns(expectedDto);

            // Act
            var result = _employeeService.GetEmployee(employeeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Email, result!.Email);
        }

        [Fact]
        public void GetEmployee_ByEmail_ReturnsMappedEmployeeDto()
        {
            // Arrange
            string email = "john@example.com";
            var employee = new Employee { EmployeeId = 1, Email = email, Created = DateTime.Now };
            _repoMock.Setup(r => r.GetEmployee(email)).Returns(employee);

            var expectedDto = new EmployeeDto { EmployeeId = 1, Email = email };
            _mapperMock.Setup(m => m.Map<EmployeeDto>(employee)).Returns(expectedDto);

            // Act
            var result = _employeeService.GetEmployee(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Email, result!.Email);
        }

        [Fact]
        public void GetEmployees_ReturnsMappedEmployeeDtos()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { EmployeeId = 1, Email = "john@example.com", Created = DateTime.Now },
                new Employee { EmployeeId = 2, Email = "jane@example.com", Created = DateTime.Now }
            };
            _repoMock.Setup(r => r.GetEmployees()).Returns(employees);

            var expectedDtos = new List<EmployeeDto>
            {
                new EmployeeDto { EmployeeId = 1, Email = "john@example.com" },
                new EmployeeDto { EmployeeId = 2, Email = "jane@example.com" }
            };
            _mapperMock.Setup(m => m.Map<List<EmployeeDto>>(employees)).Returns(expectedDtos);

            // Act
            var result = _employeeService.GetEmployees();

            // Assert
            Assert.Equal(expectedDtos.Count, result.Count);
            Assert.Equal(expectedDtos[1].Email, result[1].Email);
        }

        #endregion
    }
}

