
namespace CarFlipper.API.DTO
{
    public class AdDTO
    {
        public int AdId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; }
        public string Source { get; set; }
        public string Region { get; set; }
        public int Price { get; set; }
        public int Milage { get; set; }
        public int ModelYear { get; set; }
        public string Fuel { get; set; }
        public string Gearbox { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
