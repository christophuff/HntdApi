namespace HntdApi.DTOs;

public class CreateHauntedLocationDTO
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Description { get; set; }
    public string? History { get; set; }
    public string? ImageUrl { get; set; }

    public int LocationTypeId { get; set; }
    public int ActivityLevelId { get; set; }

    public List<int>? ParanormalActivityIds { get; set; }
}