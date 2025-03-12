using UserApi.Repository.Model;

namespace UserApi.Repository
{
    public class InMemoryUserRespository : IUserRepository
    {
        private readonly List<SystemUser> _users = new();
        private int _nextId = 1;

        public void Add(SystemUser user)
        {
            // If the user's Id is 0 (default), assign a new auto-incremented Id.
            if (user.UserId == 0)
            {
                user.UserId = _nextId++;
            }
            _users.Add(user);
        }

        public SystemUser? GetUser(int userId)
        {
           return _users.FirstOrDefault(u => u.UserId == userId);
        }

        public List<SystemUser> GetUsers()
        {
            return _users;
        }
        public SystemUser? GetUser(string mail)
        {
            return _users.FirstOrDefault(u => u.Email == mail);
        }

        public List<SystemUser> GetActiveUsers()
        {
            return _users.FindAll(u => u.Deleted == null);
        }

        public bool HardDelete(int userId)
        {
            var user = _users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                _users.Remove(user);
                return true;
            }
            return false;
        }

        public bool SoftDelete(int userId)
        {
            var index = _users.FindIndex(u => u.UserId == userId);
            if(index == -1)
            {
                return false;
            }
            _users[index].Deleted = DateTime.Now;
            return true;
        }

        public bool SoftDelete(string mail)
        {
            var index = _users.FindIndex(u => u.Email == mail);
            if (index == -1)
            {
                return false;
            }
            _users[index].Deleted = DateTime.Now;
            return true;
        }
    }
}
