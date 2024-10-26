using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PostGroup
    {
        public required string GroupId { get; set; }     
        public required string PostId { get; set; }         
    }
}
