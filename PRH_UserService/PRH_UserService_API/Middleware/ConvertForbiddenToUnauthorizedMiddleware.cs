using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace PRH_UserService_API.Middleware
{
    public class ConvertForbiddenToUnauthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        public ConvertForbiddenToUnauthorizedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
        }
    }
}
