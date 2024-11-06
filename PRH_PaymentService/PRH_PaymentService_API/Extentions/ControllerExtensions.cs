using Application.Commons;
using Microsoft.AspNetCore.Mvc;

public static class ControllerExtensions
{
    public static IActionResult ToActionResult<T>(this BaseResponse<T> response)
    {
        return new ObjectResult(response)
        {
            StatusCode = response.StatusCode
        };
    }

    public static IActionResult ToActionResult<T>(this DetailBaseResponse<T> response)
    {
        return new ObjectResult(response)
        {
            StatusCode = response.StatusCode
        };
    }
}