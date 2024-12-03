﻿using System.Linq.Expressions;
using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IPostRepository : IReadRepository<Post>, ICreateRepository<Post>, IUpdateRepository<Post>, IDeleteRepository
    {
        Task<IEnumerable<Post>> GetByUserIdAsync(string userId);
        Task<bool> ExistsAsync(string postId);
        Task<IEnumerable<Post>> GetRecommendedPostsAsync(string userId, int pageNumber, int pageSize);
        Task<IEnumerable<Post>> GetRandomPostsAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Post>> GetsPostByPropertyPagingAsync(Expression<Func<Post, bool>> predicate, int pageNumber, int pageSize);
    }
}
