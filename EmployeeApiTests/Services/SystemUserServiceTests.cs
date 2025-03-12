using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserApi.Repository.DTOs;
using UserApi.Repository.Model;
using UserApi.Repository;
using UserApi.Services;

namespace EmployeeApiTests.Services
{
    public class SystemUserServiceTests
    {
        private readonly Mock<IUserRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly SystemUserService _service;

        public SystemUserServiceTests()
        {
            _repoMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new SystemUserService(_repoMock.Object, _mapperMock.Object);
        }

        #region Get Methods

        [Fact]
        public void GetActiveUsers_ReturnsMappedDtos()
        {
            // Arrange
            var users = new List<SystemUser>
            {
                new SystemUser { UserId = 1, Email = "active1@example.com", Deleted = null },
                new SystemUser { UserId = 2, Email = "active2@example.com", Deleted = null }
            };
            _repoMock.Setup(r => r.GetActiveUsers()).Returns(users);

            var dtos = new List<SystemUserDto>
            {
                new SystemUserDto { UserId = 1, Email = "active1@example.com" },
                new SystemUserDto { UserId = 2, Email = "active2@example.com" }
            };
            _mapperMock.Setup(m => m.Map<List<SystemUserDto>>(users)).Returns(dtos);

            // Act
            var result = _service.GetActiveUsers();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("active1@example.com", result[0].Email);
        }

        [Fact]
        public void GetUserById_UserExists_ReturnsUser()
        {
            // Arrange
            int userId = 1;
            var user = new SystemUser { UserId = userId, Email = "user@example.com" };
            _repoMock.Setup(r => r.GetUser(userId)).Returns(user);

            // Act
            var result = _service.GetUserById(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user@example.com", result!.Email);
        }

        [Fact]
        public void GetUserById_UserNotFound_ReturnsNull()
        {
            // Arrange
            int userId = 1;
            _repoMock.Setup(r => r.GetUser(userId)).Returns((SystemUser?)null);

            // Act
            var result = _service.GetUserById(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetUserByMail_UserExists_ReturnsUser()
        {
            // Arrange
            string email = "user@example.com";
            var user = new SystemUser { UserId = 1, Email = email };
            _repoMock.Setup(r => r.GetUser(email)).Returns(user);

            // Act
            var result = _service.GetUserByMail(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result!.Email);
        }

        [Fact]
        public void GetUserByMail_UserNotFound_ReturnsNull()
        {
            // Arrange
            string email = "user@example.com";
            _repoMock.Setup(r => r.GetUser(email)).Returns((SystemUser?)null);

            // Act
            var result = _service.GetUserByMail(email);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetUsers_ReturnsMappedDtos()
        {
            // Arrange
            var users = new List<SystemUser>
            {
                new SystemUser { UserId = 1, Email = "user1@example.com" },
                new SystemUser { UserId = 2, Email = "user2@example.com" }
            };
            _repoMock.Setup(r => r.GetUsers()).Returns(users);

            var dtos = new List<SystemUserDto>
            {
                new SystemUserDto { UserId = 1, Email = "user1@example.com" },
                new SystemUserDto { UserId = 2, Email = "user2@example.com" }
            };
            _mapperMock.Setup(m => m.Map<List<SystemUserDto>>(users)).Returns(dtos);

            // Act
            var result = _service.GetUsers();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("user1@example.com", result[0].Email);
        }

        #endregion

        #region HardDeleteUser Tests

        [Fact]
        public void HardDeleteUser_UserNotFound_ReturnsFail()
        {
            // Arrange
            int userId = 1;
            _repoMock.Setup(r => r.GetUser(userId)).Returns((SystemUser?)null);

            // Act
            var result = _service.HardDeleteUser(userId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public void HardDeleteUser_UserFound_ReturnsOk_AndCallsHardDelete()
        {
            // Arrange
            int userId = 1;
            var user = new SystemUser { UserId = userId, Email = "user@example.com" };
            _repoMock.Setup(r => r.GetUser(userId)).Returns(user);

            // Act
            var result = _service.HardDeleteUser(userId);

            // Assert
            Assert.True(result.Success);
            _repoMock.Verify(r => r.HardDelete(userId), Times.Once);
        }

        #endregion

        #region SoftDeleteUser Tests

        [Fact]
        public void SoftDeleteUser_ById_UserNotFound_ReturnsFail()
        {
            // Arrange
            int userId = 1;
            _repoMock.Setup(r => r.GetUser(userId)).Returns((SystemUser?)null);

            // Act
            var result = _service.SoftDeleteUser(userId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public void SoftDeleteUser_ById_UserFound_ReturnsOk_AndCallsHardDelete()
        {
            // Arrange
            int userId = 1;
            var user = new SystemUser { UserId = userId, Email = "user@example.com" };
            _repoMock.Setup(r => r.GetUser(userId)).Returns(user);

            // Act
            var result = _service.SoftDeleteUser(userId);

            // Assert
            Assert.True(result.Success);
            // Note: According to your implementation, this calls HardDelete.
            _repoMock.Verify(r => r.HardDelete(userId), Times.Once);
        }

        [Fact]
        public void SoftDeleteUser_ByMail_UserNotFound_ReturnsFail()
        {
            // Arrange
            string email = "user@example.com";
            _repoMock.Setup(r => r.GetUser(email)).Returns((SystemUser?)null);

            // Act
            var result = _service.SoftDeleteUser(email);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public void SoftDeleteUser_ByMail_UserFound_ReturnsOk_AndCallsSoftDelete()
        {
            // Arrange
            string email = "user@example.com";
            var user = new SystemUser { UserId = 1, Email = email };
            _repoMock.Setup(r => r.GetUser(email)).Returns(user);

            // Act
            var result = _service.SoftDeleteUser(email);

            // Assert
            Assert.True(result.Success);
            _repoMock.Verify(r => r.SoftDelete(email), Times.Once);
        }

        #endregion
    }
}
