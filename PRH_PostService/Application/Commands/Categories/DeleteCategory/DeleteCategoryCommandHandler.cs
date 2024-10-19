using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System.Net;


namespace Application.Commands.Categories.DeleteCategory
{
    public class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository) : IRequestHandler<DeleteCategoryCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.categoryId,
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                await categoryRepository.DeleteAsync(request.categoryId);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Xoá danh mục thành công!";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Lỗi !!! Không xoá được danh mục";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
