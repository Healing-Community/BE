using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Categories.AddCategory
{
    public record CreateCategoryCommand(CategoryDto CategoryDto) : IRequest<BaseResponse<string>>;
}
