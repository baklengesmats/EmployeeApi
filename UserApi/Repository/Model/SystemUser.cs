using Microsoft.AspNetCore.Http.HttpResults;

namespace UserApi.Repository.Model
{
    public class SystemUser
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
