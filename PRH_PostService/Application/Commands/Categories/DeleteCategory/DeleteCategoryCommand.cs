using Application.Commons;
using MediatR;

namespace Application.Commands.Categories.DeleteCategory
{
    public record DeleteCategoryCommand(string categoryId) : IRequest<BaseResponse<string>>;
}
