using System.ComponentModel.DataAnnotations;

namespace Application.Commons.DTOs
{
    public class GroupDto
    {
        [Required(ErrorMessage = "Tên nhóm không được bỏ trống.")]
        public string GroupName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsAutoApprove { get; set; }
        public int GroupVisibility { get; set; } = 0; // 0: Public, 1: Private
        public int MemberLimit { get; set; } = 50;
    }
}
