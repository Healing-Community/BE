﻿using Application.Interfaces.GenericRepository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repository
{
    public interface IReportTypeRepository : IReadRepository<ReportType>, ICreateRepository<ReportType>, IUpdateRepository<ReportType>, IDeleteRepository
    {
    }
}
