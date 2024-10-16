using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.DASS21
{
    public class Dass21
    {
        public required Guid Id { get; set; }
        public List<Category>? Dass21Categories { get; set; }
    }
}
