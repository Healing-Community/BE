using Application.Commons;
using MediatR;

namespace Application.Commands.Categories.DeleteCategory
{
    public record DeleteCategoryCommand(Guid categoryId) : IRequest<BaseResponse<string>>;
}
