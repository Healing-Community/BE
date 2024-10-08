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

namespace Application.Queries.Categories.GetCategories
{
    public class GetCategoryQueryHandler(ICategoryRepository categoryRepository) 
        : IRequestHandler<GetCategoryQuery, BaseResponse<IEnumerable<Category>>>
    {
        public async Task<BaseResponse<IEnumerable<Category>>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<Category>>
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow,
            };
            try
            {
                var categories = await categoryRepository.GetsAsync();
                response.Message = "Categories retrieved successfully";
                response.Success = true;
                response.Data = categories;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
            }
            return response;
        }
    }
}
