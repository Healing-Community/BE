using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commons.DTOs
{
    public class UserDto
    {
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int Status { get; set; }
        public string PasswordHash { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int RoleId { get; set; }
    }
}
