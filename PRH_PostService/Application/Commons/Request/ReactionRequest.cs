﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commons.Request
{
    public class ReactionRequest
    {
        public Guid ReactionId { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public Guid ReactionTypeId { get; set; }

    }
}
