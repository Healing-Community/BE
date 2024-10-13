using Application.Commons;
using Microsoft.AspNetCore.Mvc;

namespace PRH_UserService_API.Extentions;

public static class ControllerExtensions
{
    public static IActionResult ToActionResult<T>(this BaseResponse<T> response)
    {
        return new ObjectResult(response)
        {
            StatusCode = response.StatusCode
        };
    }
}