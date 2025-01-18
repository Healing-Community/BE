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
        [HttpGet("get-all")]
        public async Task<IActionResult> GetsCategory()
        {
            var response = await sender.Send(new GetCategoryQuery());
            return response.ToActionResult();
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await sender.Send(new GetCategoryByIdQuery(id));
            return response.ToActionResult();
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory(CategoryDto category)
        {
            var response = await sender.Send(new CreateCategoryCommand(category));
            return response.ToActionResult();
        }

        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCategory(string id, CategoryDto category)
        {
            var response = await sender.Send(new UpdateCategoryCommand(id, category));
            return response.ToActionResult();
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var response = await sender.Send(new DeleteCategoryCommand(id));
            return response.ToActionResult();
        }
    }
}
