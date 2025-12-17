namespace HntdApi.DTOs;

public class UserDTO
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime DateCreated { get; set; }
    public string? ImageUrl { get; set; }
}