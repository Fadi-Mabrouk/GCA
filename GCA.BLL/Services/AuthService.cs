using System.Threading.Tasks;
using GCA.BLL.Helpers;
using GCA.BLL.Interfaces;
using GCA.DAL;
using GCA.Models;
using Microsoft.EntityFrameworkCore;

namespace GCA.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly GCADbContext _context;

        public AuthService(GCADbContext context)
        {
            _context = context;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;

            if (PasswordHasher.VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }

        public async Task<User> RegisterAsync(string username, string password, string fullName, UserRole role)
        {
            var existing = await _context.Users.AnyAsync(u => u.Username == username);
            if (existing) throw new System.Exception("Username already exists");

            var user = new User
            {
                Username = username,
                PasswordHash = PasswordHasher.HashPassword(password),
                FullName = fullName,
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
