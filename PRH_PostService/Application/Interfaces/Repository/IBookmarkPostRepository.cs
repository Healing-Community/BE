using System;
using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface IBookmarkPostRepository : ICreateRepository<BookmarkPost>, IReadRepository<BookmarkPost>, IUpdateRepository<BookmarkPost>, IDeleteRepository
{

}
