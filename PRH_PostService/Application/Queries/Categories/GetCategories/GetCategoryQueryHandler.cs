using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using System.Net;

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
                Errors = new List<string>()
            };
            try
            {
                var categories = await categoryRepository.GetsAsync();
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "Lấy dữ liệu thành công";
                response.Success = true;
                response.Data = categories;
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                response.Success = false;
            }
            return response;
        }
    }
}
