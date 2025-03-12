using UserApi.Repository.Model;

namespace UserApi.Repository
{
    public interface IUserRepository
    {
        public void Add(SystemUser user);
        public bool SoftDelete(int userId);
        public bool SoftDelete(string mail);
        public bool HardDelete(int userId);
        public SystemUser? GetUser(int userId);
        public SystemUser? GetUser(string mail);
        public List<SystemUser> GetUsers();
        public List<SystemUser> GetActiveUsers();

    }
}
