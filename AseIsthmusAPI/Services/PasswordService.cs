using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Templates;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Security.Cryptography;

namespace AseIsthmusAPI.Services
{
    public class PasswordService
    {
        private readonly AseItshmusContext _context;
        public PasswordService(AseItshmusContext context)
        {
            _context = context;
        }

        /// <summary>
        /// resets the password of a user that hasn't logged in
        /// </summary>
        /// <param name="passwordData"></param>
        /// <returns></returns>
        public async Task<string?> ResetPasswordUnauthenticated(GeneratePasswordDto passwordData)
        {
            var user = await _context.Users.Where(u => u.EmailAddress == passwordData.EmailAddress).FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }

            var login = await _context.Logins.Where(l => l.PersonId == user.PersonId).FirstOrDefaultAsync();
            if (login == null)
            {
                return null;
            }

            else
            {
                var newPassword = GenerateRandomPassword();
                login.Pw = EncodeHashPasswordSHA256(newPassword);
                await _context.SaveChangesAsync();
                return newPassword;
            }

        }

        /// <summary>
        /// Resets the password of a logged in user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="passwordData"></param>
        /// <returns></returns>
        public async Task<(string?,string?)> ResetPasswordAuthenticated(string id, GeneratePasswordDto passwordData) {
          
            var login = await _context.Logins
                   .Include(l => l.Person)
                   .FirstOrDefaultAsync(l => l.PersonId == id);

            if (login == null || passwordData == null)
            {
                return (null, null);
            }
            else
            {
                var newPassword = passwordData.Password;
                login.Pw = newPassword;
                await _context.SaveChangesAsync();
                return (newPassword,login.Person.EmailAddress);
            }
        }

        public static string GenerateRandomPassword(int length = 8)
        {
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string specialChars = "!@#$%^&*()_-+=<>?/";
            const string numericChars = "0123456789";

            string allChars = uppercaseChars + lowercaseChars + specialChars + numericChars;

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);

                var chars = randomBytes
                    .Select(b => allChars[b % allChars.Length])
                    .ToArray();

                return new string(chars);
            }
        }


       public string EncodeHashPasswordSHA256(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    stringBuilder.Append(hashBytes[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }
    }
}
