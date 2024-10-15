using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Comment
    {
        public Guid CommentId { get; set; }
        public Guid PostId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Post Post { get; set; } = null!;
        public Comment Parent { get; set; } 
        public ICollection<Comment> Replies { get; set; } = null!;
    }

}
