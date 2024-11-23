namespace Domain.Entities
{
    public class Group
    {
        public required string GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedByUserId { get; set; }
        public bool IsAutoApprove { get; set; } = false; // Bật/Tắt phê duyệt tự động
        public int GroupVisibility { get; set; } = 0; // 0: public, 1: private
        public int MemberLimit { get; set; } = 50; // Số lượng thành viên tối đa
        public int CurrentMemberCount { get; set; } = 0; // Số lượng thành viên hiện tại
    }
}
