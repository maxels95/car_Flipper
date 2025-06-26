using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CarFlipper.API.Enums;

namespace CarFlipper.API.Models
{
    public class Ad
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AdId { get; set; }
        
        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        public string? Engine { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        [Url]
        public string Url { get; set; } = string.Empty;

        [Required]
        public string Source { get; set; } = string.Empty; // t.ex. "Blocket", "Facebook"

        public int? RegionId { get; set; }

        public string? Location { get; set; }

        public int Price { get; set; }
        public int Milage { get; set; }
        public int ModelYear { get; set; }
        public string? Fuel { get; set; }
        public string? Gearbox { get; set; }
        public int MarketPriceId { get; set; }
        public MarketPrice? MarketPrice { get; set; }
        public bool IsUnderpriced { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RemovedAt { get; set; }
        public bool Deprecated = false;

        public Ad()
        {

        }

        public Ad(int id,
        int adId,
        string title,
        string url,
        string source,
        string? location,
        int price,
        int milage,
        int modelYear,
        string? fuel,
        string? gearbox,
        int? estimatedMarketValue,
        bool isUnderpriced,
        DateTime createdAt,
        DateTime removedAt,
        bool deprecated)
        {
            this.Id = id;
            this.AdId = adId;
            this.Title = title;
            this.Url = url;
            this.Source = source;
            this.Location = location;
            this.Price = price;
            this.Milage = milage;
            this.ModelYear = modelYear;
            this.Fuel = fuel;
            this.Gearbox = gearbox;
            this.IsUnderpriced = isUnderpriced;
            this.CreatedAt = createdAt;
            this.RemovedAt = removedAt;
            this.Deprecated = deprecated;
        }
    }

}
