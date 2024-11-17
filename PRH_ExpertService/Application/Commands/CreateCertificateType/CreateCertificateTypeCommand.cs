using Application.Commons;
using MediatR;

namespace Application.Commands.CreateCertificateType
{
    public record CreateCertificateTypeCommand(string Name, string Description, bool IsMandatory) : IRequest<DetailBaseResponse<string>>;
}