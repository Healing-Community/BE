using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using System.Net;

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
                    response.Message = "Không tìm thấy";
                    response.Errors.Add("Không tìm thấy dữ liệu nào có ID được cung cấp.");
                    return response;
                }
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Data = category;
                response.Success = true;
                response.Message = "Lấy dữ liệu thành công";
            }
            catch (Exception e)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Có lỗi xảy ra";
                response.Errors.Add(e.Message);
            }
            return response;
        }
    }
}
