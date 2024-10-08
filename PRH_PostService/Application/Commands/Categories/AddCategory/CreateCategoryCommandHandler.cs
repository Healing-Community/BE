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

namespace Application.Commands.Categories.AddCategory
{
    public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository) : IRequestHandler<CreateCategoryCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryId = NewId.NextSequentialGuid();
            var category = new Category
            {
                Id = categoryId,
                Name = request.CategoryDto.Name,
                CreateAt = DateTime.UtcNow,
            };

            var response = new BaseResponse<string>
            {
                Id = categoryId,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                await categoryRepository.Create(category);
                response.Success = true;
                response.Errors = Enumerable.Empty<string>();
                response.Message = "Category created successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors = new[] { ex.Message };
                response.Message = "Failed to create category";
            }

            return response;
        }
    }
}
