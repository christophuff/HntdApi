namespace HntdApi.DTOs;

public class UserFavoriteDTO
{
    public int Id { get; set; }
    public int UserId { get; set;}
    public UserDTO? User { get; set; }
    public int HauntedLocationId { get; set; }
    public HauntedLocationDTO? HauntedLocation { get; set; }
    public DateTime DateAdded { get; set; }
}