public class MarketPrice
{
    public int Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }

    public int ModelYearFrom { get; set; }
    public int ModelYearTo { get; set; }

    public int MilageFrom { get; set; }
    public int MilageTo { get; set; }

    public string Fuel { get; set; }
    public string Gearbox { get; set; }

    public int EstimatedPrice { get; set; }
    public int SampleSize { get; set; }  // antal annonser som beräkningen baseras på

    public DateTime UpdatedAt { get; set; }
}
