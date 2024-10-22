using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Commands.ReactionTypes.AddReactionType
{
    public class CreateReactionTypeCommandHandler(IReactionTypeRepository reactionTypeRepository) 
        : IRequestHandler<CreateReactionTypeCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateReactionTypeCommand request, CancellationToken cancellationToken)
        {
            var reactionTypeId = Ulid.NewUlid().ToString();
            var reactionType = new ReactionType
            {
                ReactionTypeId = reactionTypeId,
                Name = request.ReactionTypeDto.Name,
            };
            var response = new BaseResponse<string>
            {
                Id = reactionTypeId,
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                await reactionTypeRepository.Create(reactionType);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Tạo thành công";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Lỗi !!! Tạo thất bại";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
