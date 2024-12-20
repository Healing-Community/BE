using Application.Commons;
using Application.Commons.DTOs;
using Application.Commons.Enum;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Queries.Reactions.GetPostReactionCount;

public class GetPostReactionCountQueryHandler(IReactionRepository reactionRepository, IPostRepository postRepository) : IRequestHandler<GetPostReactionCountQuery, BaseResponse<PostReactionCountDto>>
{
    public async Task<BaseResponse<PostReactionCountDto>> Handle(GetPostReactionCountQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var post = await postRepository.GetByPropertyAsync(x => x.PostId == request.PostId.PostId);
            if (post == null)
            {
                return BaseResponse<PostReactionCountDto>.NotFound("Bài viết không tồn tại");
            }
            var reactions = await reactionRepository.GetsByPropertyAsync(x => x.PostId == request.PostId.PostId);

            var PostReactionCountDto = new PostReactionCountDto
            {
                PostId = post.PostId,
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

            return BaseResponse<PostReactionCountDto>.SuccessReturn(PostReactionCountDto);
            
        }
        catch (Exception ex)
        {
            return BaseResponse<PostReactionCountDto>.InternalServerError(ex.Message);
        }
    }
}
