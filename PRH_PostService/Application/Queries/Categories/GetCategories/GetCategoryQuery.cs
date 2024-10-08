using Application.Commons;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Categories.GetCategories
{
    public record GetCategoryQuery : IRequest<BaseResponse<IEnumerable<Category>>>;
}
