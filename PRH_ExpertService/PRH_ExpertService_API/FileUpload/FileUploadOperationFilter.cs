using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PRH_ExpertService_API.FileUpload
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var uploadFileParameters = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(FileUploadModel))
                .ToList();

            if (uploadFileParameters.Count != 0)
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(FileUploadModel), context.SchemaRepository)
                    }
                }
                };
            }
        }
    }
}