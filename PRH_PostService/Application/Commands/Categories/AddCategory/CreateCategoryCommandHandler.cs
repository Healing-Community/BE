using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using System.Net;

namespace Application.Commands.Categories.AddCategory
{
    public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository) : IRequestHandler<CreateCategoryCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryId = NewId.NextSequentialGuid();
            var category = new Category
            {
                CategoryId = categoryId,
                Name = request.CategoryDto.Name,
                CreateAt = DateTime.UtcNow,
            };
            var response = new BaseResponse<string>
            {
                Id = categoryId,
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                await categoryRepository.Create(category);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Tạo danh mục thành công!";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Lỗi !!! Không tạo được danh mục";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
