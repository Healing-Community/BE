﻿using Application.Interfaces.GenericRepository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repository
{
    public interface IRoleRepository : IReadRepository<Role>, ICreateRepository<Role>, IUpdateRepository<Role>, IDeleteRepository
    {
        public Task<string> GetRoleNameById(int roleId);
    }
}
