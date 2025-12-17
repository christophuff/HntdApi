namespace HntdApi.Models;

public class ParanormalActivity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? IconName { get; set; }
}