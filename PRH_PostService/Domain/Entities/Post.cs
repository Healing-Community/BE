using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Post
    {
        public Guid Id { get; init; }
        public Guid UserId {  get; set; }
        public Guid CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CoverImgUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string Description {  get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime CreateAt { get; init; }
        public DateTime? UpdateAt { get; set; }

        public required Category Category { get; set; }
    }
}
