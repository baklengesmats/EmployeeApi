using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserApi.Repository.Model;
using UserApi.Repository;

namespace EmployeeApiTests.Respository
{
    public class InMemoryUserTests
    {
        [Fact]
        public void Add_AssignsNewId_WhenUserIdIsZero()
        {
            // Arrange
            var repository = new InMemoryUserRespository();
            var user = new SystemUser
            {
                UserId = 0,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PasswordHash = "hash",
                Role = 1,
                Created = DateTime.Now
            };

            // Act
            repository.Add(user);

            // Assert
            Assert.True(user.UserId > 0);
            var retrievedUser = repository.GetUser(user.UserId);
            Assert.NotNull(retrievedUser);
            Assert.Equal("john@example.com", retrievedUser!.Email);
        }

        [Fact]
        public void GetUser_ByEmail_ReturnsCorrectUser()
        {
            // Arrange
            var repository = new InMemoryUserRespository();
            var user = new SystemUser
            {
                UserId = 0,
                FirstName = "Bob",
                LastName = "Brown",
                Email = "bob@example.com",
                PasswordHash = "hash",
                Role = 1,
                Created = DateTime.Now
            };
            repository.Add(user);

            // Act
            var retrievedUser = repository.GetUser("bob@example.com");

            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal("Bob", retrievedUser!.FirstName);
        }

        [Fact]
        public void GetUsers_ReturnsAllAddedUsers()
        {
            // Arrange
            var repository = new InMemoryUserRespository();
            repository.Add(new SystemUser { UserId = 0, FirstName = "User1", Email = "user1@example.com", PasswordHash = "hash", Role = 1, Created = DateTime.Now });
            repository.Add(new SystemUser { UserId = 0, FirstName = "User2", Email = "user2@example.com", PasswordHash = "hash", Role = 1, Created = DateTime.Now });

            // Act
            var users = repository.GetUsers();

            // Assert
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public void GetActiveUsers_ReturnsOnlyActiveUsers()
        {
            // Arrange
            var repository = new InMemoryUserRespository();
            var activeUser = new SystemUser { UserId = 0, FirstName = "Active", Email = "active@example.com", PasswordHash = "hash", Role = 1, Created = DateTime.Now, Deleted = null };
            var deletedUser = new SystemUser { UserId = 0, FirstName = "Deleted", Email = "deleted@example.com", PasswordHash = "hash", Role = 1, Created = DateTime.Now, Deleted = DateTime.Now };

            repository.Add(activeUser);
            repository.Add(deletedUser);

            // Act
            var activeUsers = repository.GetActiveUsers();

            // Assert
            Assert.Single(activeUsers);
            Assert.Equal("active@example.com", activeUsers.First().Email);
        }

        [Fact]
        public void HardDelete_RemovesUserAndReturnsTrue()
        {
            // Arrange
            var repository = new InMemoryUserRespository();
            var user = new SystemUser { UserId = 0, FirstName = "Delete", Email = "delete@example.com", PasswordHash = "hash", Role = 1, Created = DateTime.Now };
            repository.Add(user);

            // Act
            bool result = repository.HardDelete(user.UserId);

            // Assert
            Assert.True(result);
            var retrievedUser = repository.GetUser(user.UserId);
            Assert.Null(retrievedUser);
        }

        [Fact]
        public void HardDelete_ReturnsFalse_WhenUserNotFound()
        {
            // Arrange
            var repository = new InMemoryUserRespository();

            // Act
            bool result = repository.HardDelete(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void SoftDelete_ById_SetsDeletedAndReturnsTrue()
        {
            // Arrange
            var repository = new InMemoryUserRespository();
            var user = new SystemUser { UserId = 0, FirstName = "Soft", Email = "soft@example.com", PasswordHash = "hash", Role = 1, Created = DateTime.Now };
            repository.Add(user);

            // Act
            bool result = repository.SoftDelete(user.UserId);

            // Assert
            Assert.True(result);
            var retrievedUser = repository.GetUser(user.UserId);
            Assert.NotNull(retrievedUser);
            Assert.NotNull(retrievedUser!.Deleted);
        }

        [Fact]
        public void SoftDelete_ById_ReturnsFalse_WhenUserNotFound()
        {
            // Arrange
            var repository = new InMemoryUserRespository();

            // Act
            bool result = repository.SoftDelete(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void SoftDelete_ByEmail_SetsDeletedAndReturnsTrue()
        {
            // Arrange
            var repository = new InMemoryUserRespository();
            var user = new SystemUser { UserId = 0, FirstName = "Email", Email = "email@example.com", PasswordHash = "hash", Role = 1, Created = DateTime.Now };
            repository.Add(user);

            // Act
            bool result = repository.SoftDelete("email@example.com");

            // Assert
            Assert.True(result);
            var retrievedUser = repository.GetUser("email@example.com");
            Assert.NotNull(retrievedUser);
            Assert.NotNull(retrievedUser!.Deleted);
        }

        [Fact]
        public void SoftDelete_ByEmail_ReturnsFalse_WhenUserNotFound()
        {
            // Arrange
            var repository = new InMemoryUserRespository();

            // Act
            bool result = repository.SoftDelete("nonexistent@example.com");

            // Assert
            Assert.False(result);
        }
    }
}
