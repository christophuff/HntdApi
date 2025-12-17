namespace HntdApi.DTOs;

public class HauntedLocationDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Description { get; set; }
    public string? History { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime DateAdded { get; set; }

    public int LocationTypeId { get; set; }
    public LocationTypeDTO? LocationType { get; set; }
    public int ActivityLevelId { get; set; }
    public ActivityLevelDTO? ActivityLevel { get; set; }
    public int UserId { get; set; }
    public UserDTO? User { get; set; }

    public List<ParanormalActivityDTO>? ParanormalActivities { get; set; }
}