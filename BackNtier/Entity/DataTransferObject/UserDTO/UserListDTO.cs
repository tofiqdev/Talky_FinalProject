using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DataTransferObject.UserDTO
{
    public class UserListDTO
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Avatar { get; set; }
        public string? Bio { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeen { get; set; }
        public string PasswordHash { get; set; } // Required for current AuthController logic
        public DateTime CreatedAt { get; set; }
    }
}
