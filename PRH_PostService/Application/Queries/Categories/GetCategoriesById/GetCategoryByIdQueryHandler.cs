using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Categories.GetCategoriesById
{
    public class GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetCategoryByIdQuery, BaseResponse<Category>>
    {
        public async Task<BaseResponse<Category>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<Category>()
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow,
            };
            try
            {
                var category = await categoryRepository.GetByIdAsync(request.categoryId);
                response.Data = category;
                response.Success = true;
                response.Message = "Category retrieved successfully";
                response.Errors = Enumerable.Empty<string>();
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = "An error occurred";
                response.Errors = new[] { e.Message };
            }
            return response;
        }
    }
}
