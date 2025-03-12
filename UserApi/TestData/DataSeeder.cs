using System.Text.Json;
using UserApi.Repository.Model;
using UserApi.Repository;

namespace UserApi.TestData
{
    public class DataSeeder
    {
        public static void SeedAdminUsers(WebApplication app)
        {
            var repository = app.Services.GetRequiredService<IUserRepository>();
            string filePath = Path.Combine(AppContext.BaseDirectory, "TestData", "users.json");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (File.Exists(filePath))
            {
                var jsonContent = File.ReadAllText(filePath);
                var adminUsers = JsonSerializer.Deserialize<List<SystemUser>>(jsonContent, options);
                if (adminUsers is not null)
                {
                    foreach (var admin in adminUsers)
                    {
                        repository.Add(admin);
                    }
                }
            }
            else
            {
                // If the file does not exist, add a default admin user.
                var defaultAdmin = new SystemUser()
                {
                    UserId = 0,
                    FirstName = "Test",
                    LastName = "Testsson",
                    Email = "test@test.se",
                    Created = DateTime.Now,
                    Deleted = null, 
                    //Password test123
                    PasswordHash = "AQAAAAEAACcQAAAAEO8jhWm+0wIh3QFJ+mzH0h9F+3gVwJ5M1q0jRvjbuj+TfS5Lcz7W8A9dJkbs7PXfw==",
                    Role=1

                };
                repository.Add(defaultAdmin);
            }
        }

    }
}
