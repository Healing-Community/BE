using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; init; }
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int status { get; set; } = 0;
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int RoleId { get; set; }
    }
}
