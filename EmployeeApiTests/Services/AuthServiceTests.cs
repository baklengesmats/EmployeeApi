using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserApi.Repository.Model;
using UserApi.Repository;
using UserApi.Services;
using Microsoft.Extensions.Configuration;

namespace EmployeeApiTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _repoMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly SystemUserAuthService _authService;

        public AuthServiceTests()
        {
            _repoMock = new Mock<IUserRepository>();
            _configMock = new Mock<IConfiguration>();

            // Provide valid JWT configuration values.
            _configMock.Setup(c => c["Jwt:Key"]).Returns("ThisIsA32ByteLongSecretKeyForHS256!!");
            _configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

            _authService = new SystemUserAuthService(_repoMock.Object, _configMock.Object);
        }

        #region GenerateJwtToken Tests

        [Fact]
        public void GenerateJwtToken_UserNotFound_ReturnsNull()
        {
            // Arrange
            string email = "nonexistent@example.com";
            _repoMock.Setup(r => r.GetUser(email)).Returns((SystemUser?)null);

            // Act
            var token = _authService.GenerateJwtToken(email);

            // Assert
            Assert.Null(token);
        }

        [Fact]
        public void GenerateJwtToken_UserFound_ReturnsToken()
        {
            // Arrange
            string email = "user@example.com";
            var user = new SystemUser
            {
                UserId = 1,
                Email = email,
                Role = (int)SystemUserRolesEnum.Regular,
                FirstName = "John",
                LastName = "Doe",
                Created = DateTime.Now
            };
            _repoMock.Setup(r => r.GetUser(email)).Returns(user);

            // Act
            var token = _authService.GenerateJwtToken(email);

            // Assert
            Assert.NotNull(token);
            Assert.IsType<string>(token);
            Assert.NotEmpty(token);
        }

        #endregion

        #region Register Tests

        [Fact]
        public void Register_CallsRepositoryAdd_WithHashedPassword()
        {
            // Arrange
            string email = "newuser@example.com";
            string firstname = "New";
            string lastname = "User";
            string password = "test123";

            SystemUser? addedUser = null;
            _repoMock.Setup(r => r.Add(It.IsAny<SystemUser>()))
                     .Callback<SystemUser>(u => addedUser = u);

            // Act
            _authService.Register(firstname, lastname, email, password, SystemUserRolesEnum.Regular);

            // Assert
            _repoMock.Verify(r => r.Add(It.IsAny<SystemUser>()), Times.Once);
            Assert.NotNull(addedUser);
            Assert.Equal(email, addedUser!.Email);
            Assert.NotEqual(password, addedUser.PasswordHash); // The password should be hashed.
        }

        #endregion

        #region ValidateLogin Tests

        [Fact]
        public void ValidateLogin_UserNotFound_ReturnsFalse()
        {
            // Arrange
            string email = "nonexistent@example.com";
            _repoMock.Setup(r => r.GetUser(email)).Returns((SystemUser?)null);

            // Act
            var result = _authService.ValidateLogin(email, "anyPassword");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateLogin_ValidPassword_ReturnsTrue()
        {
            // Arrange
            string email = "user@example.com";
            string password = "test123";
            var user = new SystemUser
            {
                UserId = 1,
                Email = email,
                Role = (int)SystemUserRolesEnum.Regular,
                FirstName = "John",
                LastName = "Doe",
                Created = DateTime.Now,
            };

            // Simulate registration by hashing the password.
            var hasher = new PasswordHasher<SystemUser>();
            user.PasswordHash = hasher.HashPassword(user, password);

            _repoMock.Setup(r => r.GetUser(email)).Returns(user);

            // Act
            var result = _authService.ValidateLogin(email, password);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateLogin_InvalidPassword_ReturnsFalse()
        {
            // Arrange
            string email = "user@example.com";
            string correctPassword = "test123";
            string wrongPassword = "wrongpassword";
            var user = new SystemUser
            {
                UserId = 1,
                Email = email,
                Role = (int)SystemUserRolesEnum.Regular,
                FirstName = "John",
                LastName = "Doe",
                Created = DateTime.Now,
            };

            var hasher = new PasswordHasher<SystemUser>();
            user.PasswordHash = hasher.HashPassword(user, correctPassword);

            _repoMock.Setup(r => r.GetUser(email)).Returns(user);

            // Act
            var result = _authService.ValidateLogin(email, wrongPassword);

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}
