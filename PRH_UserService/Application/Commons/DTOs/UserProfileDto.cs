using Application.Commons.DTOs;

public class UserProfileDto
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
    public string Descrtiption { get; set; } = string.Empty;
    public SocialLinkDto SocialLink { get; set; } = new SocialLinkDto();
    public string CreatedAt { get; set; } = string.Empty;
    public string UpdatedAt { get; set; } = string.Empty;
}
