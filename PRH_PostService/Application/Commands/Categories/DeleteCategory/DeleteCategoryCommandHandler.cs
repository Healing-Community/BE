using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            };

            try
            {
                await categoryRepository.DeleteAsync(request.categoryId);
                response.Success = true;
                response.Message = "Category deleted successfully";
                response.Errors = new List<string>();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to delete category";
                response.Errors = new[] { ex.Message };
            }
            return response;
        }
    }
}
