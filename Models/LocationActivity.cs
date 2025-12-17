namespace HntdApi.Models;

public class LocationActivity
{
    public int Id { get; set; }
    public required int HauntedLocationId { get; set; }
    public HauntedLocation? HauntedLocation { get; set; }
    public required int ParanormalActivityId { get; set; }
    public ParanormalActivity? ParanormalActivity { get; set; }
}