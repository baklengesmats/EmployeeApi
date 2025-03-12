using AutoMapper;
using UserApi.Repository.DTOs;
using UserApi.Repository.Model;

namespace UserApi.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<SystemUser, SystemUserDto>();
            CreateMap<Employee, EmployeeDto>();
        }
    }
}
