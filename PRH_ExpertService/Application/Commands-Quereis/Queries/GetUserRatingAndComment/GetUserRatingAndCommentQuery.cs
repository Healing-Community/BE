using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Queries.GetUserRatingAndComment
{
    public class GetUserRatingAndCommentQuery : IRequest<BaseResponse<UserRatingAndCommentDto>>
    {
        public string AppointmentId { get; set; }
    }
}
