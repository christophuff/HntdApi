namespace HntdApi.DTOs;

public class LocationActivityDTO
{
    public int Id { get; set; }
    public int HauntedLocationId { get; set; }
    public HauntedLocationDTO? HauntedLocation { get; set; }
    public int ParanormalActivityId { get; set; }
    public ParanormalActivityDTO? ParanormalActivity { get; set; }
}