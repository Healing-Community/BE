using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Categories.GetCategoriesById
{
    public record GetCategoryByIdQuery(Guid categoryId) : IRequest<BaseResponse<Category>>;
}
