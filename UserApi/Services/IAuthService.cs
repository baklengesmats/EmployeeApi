using UserApi.Repository.Model;

namespace UserApi.Services
{
    public interface IAuthService
    {
        public void Register(string firstname, string lastname, string email, string password, SystemUserRolesEnum role);
        public bool ValidateLogin(string mail, string password);
        public string? GenerateJwtToken(string mail);
    }
}
