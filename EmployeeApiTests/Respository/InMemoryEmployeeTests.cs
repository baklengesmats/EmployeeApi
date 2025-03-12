using UserApi.Repository;
using UserApi.Repository.Model;

namespace EmployeeApiTests.Respository
{
    public class InMemoryEmployeeTests
    {
        [Fact]
        public void Add_AssignsNewId_WhenEmployeeIdIsZero()
        {
            // Arrange
            var repository = new InMemoryEmployeeRespository();
            var employee = new Employee
            {
                EmployeeId = 0,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            // Act
            repository.Add(employee);

            // Assert
            Assert.True(employee.EmployeeId == 1);
        }

        [Fact]
        public void SoftDelete_ByEmployeeId_SetsDeleted()
        {
            // Arrange
            var repository = new InMemoryEmployeeRespository();
            var employee = new Employee { Email = "john.doe@example.com" };
            repository.Add(employee);

            // Act
            bool result = repository.SoftDelete(employee.EmployeeId);

            // Assert
            Assert.True(result);
            var retrievedEmployee = repository.GetEmployee(employee.EmployeeId);
            Assert.NotNull(retrievedEmployee);
            Assert.NotNull(retrievedEmployee!.Deleted);
        }

        [Fact]
        public void SoftDelete_ByEmployeeId_WhenNotFound_ReturnsFalse()
        {
            // Arrange
            var repository = new InMemoryEmployeeRespository();

            // Act
            bool result = repository.SoftDelete(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void SoftDelete_ByEmail_SetsDeleted()
        {
            // Arrange
            var repository = new InMemoryEmployeeRespository();
            var employee = new Employee { Email = "jane.doe@example.com" };
            repository.Add(employee);

            // Act
            bool result = repository.SoftDelete(employee.Email);

            // Assert
            Assert.True(result);
            var retrievedEmployee = repository.GetEmployee(employee.Email);
            Assert.NotNull(retrievedEmployee);
            Assert.NotNull(retrievedEmployee!.Deleted);
        }

        [Fact]
        public void SoftDelete_ByEmail_WhenNotFound_ReturnsFalse()
        {
            // Arrange
            var repository = new InMemoryEmployeeRespository();

            // Act
            bool result = repository.SoftDelete("nonexistent@example.com");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Reactive_ByEmployeeId_ClearsDeleted()
        {
            // Arrange
            var repository = new InMemoryEmployeeRespository();
            var employee = new Employee { Email = "active@example.com", Deleted = DateTime.Now };
            repository.Add(employee);

            // Act
            bool result = repository.Reactive(employee.EmployeeId);

            // Assert
            Assert.True(result);
            var retrievedEmployee = repository.GetEmployee(employee.EmployeeId);
            Assert.NotNull(retrievedEmployee);
            Assert.Null(retrievedEmployee!.Deleted);
        }

        [Fact]
        public void Reactive_ByEmployeeId_WhenNotFound_ReturnsFalse()
        {
            // Arrange
            var repository = new InMemoryEmployeeRespository();

            // Act
            bool result = repository.Reactive(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Reactive_ByEmail_ClearsDeleted()
        {
            // Arrange
            var repository = new InMemoryEmployeeRespository();
            var employee = new Employee { Email = "reactive@example.com", Deleted = DateTime.Now };
            repository.Add(employee);

            // Act
            bool result = repository.Reactive(employee.Email);

            // Assert
            Assert.True(result);
            var retrievedEmployee = repository.GetEmployee(employee.Email);
            Assert.NotNull(retrievedEmployee);
            Assert.Null(retrievedEmployee!.Deleted);
        }

        [Fact]
        public void Reactive_ByEmail_WhenNotFound_ReturnsFalse()
        {
            // Arrange
            var repository = new InMemoryEmployeeRespository();

            // Act
            bool result = repository.Reactive("nonexistent@example.com");

            // Assert
            Assert.False(result);
        }
    }
}