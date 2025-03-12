using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserApi.Repository;
using UserApi.Repository.Model;

namespace UserApi.Services
{
    public class SystemUserAuthService : IAuthService
    {
        private readonly IUserRepository _repository;
        private readonly PasswordHasher<SystemUser> _passwordHasher = new();
        private readonly IConfiguration _configuration;

        public SystemUserAuthService(IUserRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public string? GenerateJwtToken(string mail)
        {
            var user = _repository.GetUser(mail);
            if(user is null)
            {
                //User mail not found!
                return null; 
            }
            var userRole = (SystemUserRolesEnum)user.Role;

            //Get enum description name
            DescriptionAttribute attribute = userRole.GetType()
                .GetField(userRole.ToString())
                ?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault() as DescriptionAttribute;

            var roleName = attribute == null ? userRole.ToString() : attribute.Description;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, mail),
                new Claim(ClaimTypes.Role, roleName)
            };

            // Fetch key from configure
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Generate JWT-token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }

        public void Register(string firstname, string lastname, string email, string password, SystemUserRolesEnum role)
        {
            var user = new SystemUser() {
                FirstName = firstname, 
                LastName = lastname, 
                Created = DateTime.Now,
                Deleted = null,
                Email = email,
                PasswordHash = password,
                Role = (int)role,
                UserId = 0
            };
            var hashedPassword = _passwordHasher.HashPassword(user, password);

            // Updated instance with the hashed password
            user.PasswordHash = hashedPassword;
            _repository.Add(user);
        }

        public bool ValidateLogin(string mail, string password)
        {
            var user = _repository.GetUser(mail);
            if (user == null)
            {
                return false;
            }
            var testHash = _passwordHasher.HashPassword(user, password);
            var decodedBytes = Convert.FromBase64String("AQAAAAIAAYagAAAAEG04ajU/pVyIT6S7ktYtcNJE5k/8uyEGeh+9lMyGYNL9Qu3NK5JCUrCuUNx2hdf15A==");
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
