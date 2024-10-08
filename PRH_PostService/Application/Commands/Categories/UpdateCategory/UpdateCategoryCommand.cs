using Application.Commons;
using Application.Commons.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Categories.UpdateCategory
{
    public record UpdateCategoryCommand(Guid categoryId, CategoryDto CategoryDto) : IRequest<BaseResponse<string>>;
}
