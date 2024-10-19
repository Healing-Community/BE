using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Categories.GetCategories
{
    public record GetCategoryQuery : IRequest<BaseResponse<IEnumerable<Category>>>;
}
