using System.ComponentModel.DataAnnotations;

namespace GCA.Models
{
    public class User : BaseEntity
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public UserRole Role { get; set; }
    }
}
