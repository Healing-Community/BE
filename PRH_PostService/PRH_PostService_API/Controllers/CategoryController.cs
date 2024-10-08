﻿using Application.Commands.Categories.AddCategory;
using Application.Commands.Categories.DeleteCategory;
using Application.Commands.Categories.UpdateCategory;
using Application.Commands.Posts.AddPost;
using Application.Commands.Posts.DeletePost;
using Application.Commands.Posts.UpdatePost;
using Application.Commons.DTOs;
using Application.Queries.Categories.GetCategories;
using Application.Queries.Categories.GetCategoriesById;
using Application.Queries.Posts.GetPosts;
using Application.Queries.Posts.GetPostsById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            return Ok(response);
        }

        [HttpGet("get-by-id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await sender.Send(new GetCategoryByIdQuery(id));
            return Ok(category);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory(CategoryDto category)
        {
            var addedCategory = await sender.Send(new CreateCategoryCommand(category));
            return Ok(addedCategory);
        }

        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> UpdateCategory(Guid id, CategoryDto category)
        {
            var updatedCategory = await sender.Send(new UpdateCategoryCommand(id, category));
            return Ok(updatedCategory);
        }

        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var response = await sender.Send(new DeleteCategoryCommand(id));
            return Ok(response);
        }


    }
}
