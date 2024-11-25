using Application.Commons.DTOs;
using Application.Interfaces.Service;


namespace Application.Services
{
    public class CommentTreeService : ICommentTreeService
    {
        public List<CommentDtoResponse> BuildCommentTree(IEnumerable<CommentDtoResponse> comments)
        {
            // Tạo lookup dictionary để tra cứu comment nhanh
            var lookup = comments.ToDictionary(c => c.CommentId, c => c);
            var rootComments = new List<CommentDtoResponse>();
            foreach (var comment in comments)
            {
                if (string.IsNullOrEmpty(comment.ParentId))
                {
                    // Nếu comment không có parent, đây là root comment
                    rootComments.Add(lookup[comment.CommentId]);
                }
                else if (lookup.ContainsKey(comment.ParentId))
                {
                    // Thêm comment vào danh sách replies của parent
                    lookup[comment.ParentId].Replies.Add(lookup[comment.CommentId]);
                }
            }
            return rootComments;
        }
    }
}
