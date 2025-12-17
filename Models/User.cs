namespace HntdApi.Models;

public class User
{
    public int Id { get; set; }
    public required string IdentityUserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required DateTime DateCreated { get; set; }
    public string? ImageUrl { get; set; }
}