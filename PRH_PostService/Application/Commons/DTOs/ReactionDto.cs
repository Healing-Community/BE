using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commons.DTOs
{
    public class ReactionDto
    {
        public Guid PostId { get; set; }
        public Guid ReactionTypeId { get; set; }
    }
}
