using UserApi.Common;
using UserApi.Repository.DTOs;
using UserApi.Repository.Model;

namespace UserApi.Services
{
    public interface ISystemUserService
    {
        public SystemUser? GetUserById(int userId);
        public SystemUser? GetUserByMail(string mail);
        public List<SystemUserDto> GetUsers();
        public List<SystemUserDto> GetActiveUsers();
        public OperationResult SoftDeleteUser(string mail);
        public OperationResult SoftDeleteUser(int userId);
        public OperationResult HardDeleteUser(int userId);
    }
}
