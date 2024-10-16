﻿using Application.Interfaces.GenericRepository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repository
{
    public interface IReactionRepository : IReadRepository<Reaction>, ICreateRepository<Reaction>, IUpdateRepository<Reaction>, IDeleteRepository
    {
    }
}
