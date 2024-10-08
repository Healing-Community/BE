using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commons.DTOs
{
    public class CategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime CreateAt { get; init; }
        public DateTime? UpdateAt { get; set; }
    }
}
