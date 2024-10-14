using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                Errors = new List<string>()
            };
            try
            {
                var category = await categoryRepository.GetByIdAsync(request.categoryId);

                if (category == null)
                {
                    response.Success = false;
                    response.Message = "Category not found";
                    response.Errors.Add("No category found with the provided ID.");
                    return response;
                }
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Data = category;
                response.Success = true;
                response.Message = "Category retrieved successfully";
            }
            catch (Exception e)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "An error occurred";
                response.Errors.Add(e.Message);
            }
            return response;
        }
    }
}
