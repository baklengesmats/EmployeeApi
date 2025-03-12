using AutoMapper;
using UserApi.Common;
using UserApi.Repository;
using UserApi.Repository.DTOs;
using UserApi.Repository.Model;

namespace UserApi.Services
{
    public class SystemUserService : ISystemUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        public SystemUserService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public List<SystemUserDto> GetActiveUsers()
        {
            var users = _repository.GetActiveUsers();
            return _mapper.Map<List<SystemUserDto>>(users);
        }

        public SystemUser? GetUserById(int userId)
        {
            var user = _repository.GetUser(userId);
            return user;
        }

        public SystemUser? GetUserByMail(string mail)
        {
            var user = _repository.GetUser(mail);
            return user;
        }

        public List<SystemUserDto> GetUsers()
        {
            var users = _repository.GetUsers();
            return _mapper.Map<List<SystemUserDto>>(users);
        }

        public OperationResult HardDeleteUser(int userId)
        {
            var user = _repository.GetUser(userId);
            if(user is null)
            {
                return OperationResult.Fail("User not found", 404);
            }
            _repository.HardDelete(userId);
            return OperationResult.Ok();
        }

        public OperationResult SoftDeleteUser(int userId)
        {
            var user = _repository.GetUser(userId);
            if (user is null)
            {
                return OperationResult.Fail("User not found", 404);
            }
            _repository.HardDelete(userId);
            return OperationResult.Ok();
        }

        public OperationResult SoftDeleteUser(string mail)
        {
            var user = _repository.GetUser(mail);
            if (user is null)
            {
                return OperationResult.Fail("User not found", 404);
            }
            _repository.SoftDelete(mail);
            return OperationResult.Ok();
        }
    }
}
