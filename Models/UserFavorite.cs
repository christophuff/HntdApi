namespace HntdApi.Models;

public class UserFavorite
{
    public int Id { get; set; }
    public required int UserId { get; set;}
    public User? User { get; set; }
    public required int HauntedLocationId { get; set; }
    public HauntedLocation? HauntedLocation { get; set; }
    public required DateTime DateAdded { get; set; }
}