using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Categories.UpdateCategory
{
    public class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository) : IRequestHandler<UpdateCategoryCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.categoryId,
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };

            try
            {
                var existingCategory = await categoryRepository.GetByIdAsync(request.categoryId);
                var updatedCategory = new Category
                {
                    Id = request.categoryId,
                    Name = request.CategoryDto.Name,
                    UpdateAt = DateTime.UtcNow,
                };
                await categoryRepository.Update(request.categoryId, updatedCategory);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Category updated successfully";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Failed to update category";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
