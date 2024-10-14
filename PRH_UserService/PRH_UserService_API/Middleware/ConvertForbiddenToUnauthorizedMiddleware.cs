﻿using System.Net;

namespace PRH_UserService_API.Middleware;

public class ConvertForbiddenToUnauthorizedMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        await next(context);

        if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    }
}