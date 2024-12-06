using System;
using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface IBookMarkRepository : IReadRepository<Bookmark>, ICreateRepository<Bookmark>, IUpdateRepository<Bookmark>, IDeleteRepository
{

}
