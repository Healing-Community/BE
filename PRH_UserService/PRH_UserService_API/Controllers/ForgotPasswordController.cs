using Application.Commons;
using Infrastructure.Context;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUlid;
using System.Runtime.CompilerServices;

namespace PRH_UserService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController(IRedisContext redisContext) : ControllerBase
    {
        [HttpGet("set-value")]
        public async Task<IActionResult> SetValue(string value)
        {
            var response = new BaseResponse<string>()
            {
                Id = Ulid.NewUlid().ToString(),
                Data = value,
                Message = "Set value successfully",
                Success = true,
                Errors = null,
                StatusCode = StatusCodes.Status200OK,
                Timestamp = DateTime.UtcNow
            };
            TimeSpan expiry = TimeSpan.FromMinutes(1);
            bool result = await redisContext.SaveEntityToHashAsync("DemoResponse",response, expiry);
            return Ok(result);
        }
    }
}
