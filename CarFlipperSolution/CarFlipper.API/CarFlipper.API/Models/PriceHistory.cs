using System.ComponentModel.DataAnnotations;

public class PriceHistory
{
    public int Id { get; set; }

    [Required]
    public string Make { get; set; } = string.Empty;

    [Required]
    public string Model { get; set; } = string.Empty;

    public int? Year { get; set; }

    public int? Mileage { get; set; }

    [Range(0, int.MaxValue)]
    public int Price { get; set; }

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}