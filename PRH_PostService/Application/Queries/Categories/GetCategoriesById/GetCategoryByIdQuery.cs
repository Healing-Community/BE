using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Categories.GetCategoriesById
{
    public record GetCategoryByIdQuery(string categoryId) : IRequest<BaseResponse<Category>>;
}
