namespace HntdApi.Models;

public class HauntedLocation
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Description { get; set; }
    public string? History { get; set; }
    public string? ImageUrl { get; set; }
    public required DateTime DateAdded { get; set; }

    // Foreign Key Properties
    public required int LocationTypeId { get; set; }
    public LocationType? LocationType { get; set; }
    public required int ActivityLevelId { get; set; }
    public ActivityLevel? ActivityLevel { get; set; }
    public required int UserId { get; set; }
    public User? User { get; set; }
}