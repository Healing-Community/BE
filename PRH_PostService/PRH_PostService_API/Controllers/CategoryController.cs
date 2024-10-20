using Application.Commands.Categories.AddCategory;
using Application.Commands.Categories.DeleteCategory;
using Application.Commands.Categories.UpdateCategory;
using Application.Commons.DTOs;
using Application.Queries.Categories.GetCategories;
using Application.Queries.Categories.GetCategoriesById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_PostService_API.Extentions;

namespace PRH_PostService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class categoryController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "User")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetsCategory()
        {
            var response = await sender.Send(new GetCategoryQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpGet("get-by-id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await sender.Send(new GetCategoryByIdQuery(id));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory(CategoryDto category)
        {
            var response = await sender.Send(new CreateCategoryCommand(category));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> UpdateCategory(Guid id, CategoryDto category)
        {
            var response = await sender.Send(new UpdateCategoryCommand(id, category));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var response = await sender.Send(new DeleteCategoryCommand(id));
            return response.ToActionResult();
        }


    }
}
