using System.Threading.Tasks;
using GCA.Models;

namespace GCA.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string username, string password);
        Task<User> RegisterAsync(string username, string password, string fullName, UserRole role);
    }
}
