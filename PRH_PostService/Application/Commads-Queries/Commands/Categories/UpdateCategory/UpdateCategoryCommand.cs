using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Categories.UpdateCategory
{
    public record UpdateCategoryCommand(string categoryId, CategoryDto CategoryDto) : IRequest<BaseResponse<string>>;
}
