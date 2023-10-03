using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AseIsthmusAPI.Services
{
    public class LoginService
    {
        private readonly AseItshmusContext _context;
        private readonly PasswordService _passwordService;
        public LoginService(AseItshmusContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<string> GetRoleDescription(int roleId)
        {
            var roleDescription = await _context.Roles
               .Include(l => l.Users)
               .FirstOrDefaultAsync(x => x.RoleId == roleId);

            if (roleDescription == null)  return null;

            return roleDescription.RoleDescription;
        }
        public async Task<Login?> GetLogin(LoginInDto login)
        {
            var loginEntity = await _context.Logins
             .Include(l => l.Person)
             .FirstOrDefaultAsync(x => x.Person.EmailAddress == login.EmailAddress);

            if (loginEntity == null)  return null;

            return loginEntity;
        }

    }
}
