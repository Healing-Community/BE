using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Reaction
    {
        public Guid ReactionId { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public Guid ReactionTypeId { get; set; }

        // Navigation properties
        public Post Post { get; set; } = null!;
        public ReactionType ReactionType { get; set; } = null!;
    }
}
