using Application.Commons.DTOs;
using Application.Commons.Enum;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commads_Queries.Queries.Reactions.GetShareReactionCount
{
    public class GetShareReactionCountQueryHandler(IReactionRepository reactionRepository, IShareRepository shareRepository)
            : IRequestHandler<GetShareReactionCountQuery, BaseResponse<PostReactionCountDto>>
    {
        public async Task<BaseResponse<PostReactionCountDto>> Handle(GetShareReactionCountQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Kiểm tra xem ShareId có tồn tại hay không
                var share = await shareRepository.GetByPropertyAsync(x => x.ShareId == request.ShareId.ShareId);
                if (share == null)
                {
                    return BaseResponse<PostReactionCountDto>.NotFound("Bài viết được chia sẻ không tồn tại");
                }

                // Lấy danh sách reactions liên quan đến ShareId
                var reactions = await reactionRepository.GetsByPropertyAsync(x => x.ShareId == request.ShareId.ShareId);

                // Tạo DTO phản hồi
                var shareReactionCountDto = new PostReactionCountDto
                {
                    ShareId = share.ShareId,
                    Like = new Like
                    {
                        LikeCount = reactions?.Count(x => x.ReactionTypeId == ((int)ReactionTypeEnum.Like).ToString()) ?? 0,
                        Icon = "👍"
                    },
                    Haha = new Haha
                    {
                        HahaCount = reactions?.Count(x => x.ReactionTypeId == ((int)ReactionTypeEnum.Haha).ToString()) ?? 0,
                        Icon = "😆"
                    },
                    Sad = new Sad
                    {
                        SadCount = reactions?.Count(x => x.ReactionTypeId == ((int)ReactionTypeEnum.Sad).ToString()) ?? 0,
                        Icon = "😢"
                    },
                    Angry = new Angry
                    {
                        AngryCount = reactions?.Count(x => x.ReactionTypeId == ((int)ReactionTypeEnum.Angry).ToString()) ?? 0,
                        Icon = "😡"
                    },
                    Love = new Love
                    {
                        LoveCount = reactions?.Count(x => x.ReactionTypeId == ((int)ReactionTypeEnum.Love).ToString()) ?? 0,
                        Icon = "❤️"
                    },
                    Wow = new Wow
                    {
                        WowCount = reactions?.Count(x => x.ReactionTypeId == ((int)ReactionTypeEnum.Wow).ToString()) ?? 0,
                        Icon = "😲"
                    },
                    Total = reactions?.Count() ?? 0
                };

                return BaseResponse<PostReactionCountDto>.SuccessReturn(shareReactionCountDto);
            }
            catch (Exception ex)
            {
                return BaseResponse<PostReactionCountDto>.InternalServerError(ex.Message);
            }
        }
    }
}
