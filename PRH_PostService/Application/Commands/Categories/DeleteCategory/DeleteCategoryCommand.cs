using Application.Commons;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Categories.DeleteCategory
{
    public record DeleteCategoryCommand(Guid categoryId) : IRequest<BaseResponse<string>>;
}
