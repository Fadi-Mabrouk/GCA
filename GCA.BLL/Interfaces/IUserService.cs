using System.Collections.Generic;
using System.Threading.Tasks;
using GCA.Models;

namespace GCA.BLL.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task DeleteUserAsync(int id);
    }
}
