using System.Collections.Generic;
using System.Threading.Tasks;
using GCA.BLL.Interfaces;
using GCA.DAL;
using GCA.Models;
using Microsoft.EntityFrameworkCore;

namespace GCA.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly GCADbContext _context;

        public UserService(GCADbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
