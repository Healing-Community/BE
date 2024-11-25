namespace Domain.Entities;
public class UserPreference
{
    public required string Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string CategoryId { get; set; }  = string.Empty;

    public Category? Category { get; set; }
}
