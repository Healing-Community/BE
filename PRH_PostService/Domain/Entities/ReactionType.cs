﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ReactionType
    {
        public Guid ReactionTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Reaction> Reactions { get; set; } 
    }

}
