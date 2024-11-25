using Application.Commons.DTOs;

namespace Application.Interfaces.Service
{
    public interface ICommentTreeService
    {
        List<CommentDtoResponse> BuildCommentTree(IEnumerable<CommentDtoResponse> comments);
    }
}
