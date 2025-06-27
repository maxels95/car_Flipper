using System.Text.Json;
using CarFlipper.API.Data;
using CarFlipper.API.DTO;
using CarFlipper.API.Enums;
using CarFlipper.API.Models;
using Microsoft.EntityFrameworkCore;

public class AdMappingService : IAdMappingService
{
    private readonly CarParserService _parser;
    private readonly IMarketPriceService _marketPriceService;
    private readonly AppDbContext _context;

    public AdMappingService(AppDbContext context, IMarketPriceService marketPriceService, CarParserService parser)
    {
        _parser = parser;
        _marketPriceService = marketPriceService;
        _context = context;
    }

    public async Task<Ad?> MapToAd(AdDTO dto)
    {
        try
        {
            var (make, model) = _parser.ParseMakeAndModel(dto.Title);
            if (make == null || model == null)
            {
                (make, model) = _parser.ParseMakeAndModel(dto.Description);
            }


            if (make == null || model == null)
                return null; // Kan ej tolkas

            var ad = new Ad
            {
                AdId = dto.AdId,
                Title = dto.Title,
                Description = dto.Description,
                Make = make,
                Model = model,
                Url = dto.Url,
                Source = dto.Source,
                Price = dto.Price,
                Milage = dto.Milage,
                ModelYear = dto.ModelYear,
                Fuel = dto.Fuel,
                Gearbox = dto.Gearbox,
                CreatedAt = dto.CreatedAt,
                IsUnderpriced = false,
                Deprecated = false
            };

            var marketPrice = await _marketPriceService.UpdateOrCreateMarketPriceAsync(ad);
            ad.MarketPriceId = marketPrice.Id;

            if ((ad.Price + 10000) < marketPrice.EstimatedPrice)
            {
                ad.IsUnderpriced = true;
            }

            if (Enum.TryParse<Region>(dto.Region, ignoreCase: true, out var regionEnum))
            {
                ad.RegionId = (int)regionEnum;
            }

            return ad;
        }
        catch
        {
            return null;
        }
    }
    

    public async Task<AdDTO?> MapToAdDTO(JsonElement jsonElement)
    {
        try
        {
            // Extract price amount and remove non-numeric characters
            var priceString = jsonElement.GetProperty("price").GetProperty("amount").GetString();
            priceString = priceString.Replace(" ", "").Replace("kr", "");
            int price = int.Parse(priceString);

            return new AdDTO
            {
                AdId = int.Parse(jsonElement.GetProperty("dealId").GetString()),
                Title = jsonElement.GetProperty("heading").GetString(),
                Description = jsonElement.GetProperty("description").GetString(),
                Url = jsonElement.GetProperty("link").GetString(),
                Source = "blocket", // Hardcoded as not in original JSON
                Region = jsonElement.GetProperty("car").GetProperty("location").GetProperty("region").GetString(),
                Price = price,
                Milage = jsonElement.GetProperty("car").GetProperty("mileage").GetInt32(),
                ModelYear = jsonElement.GetProperty("car").GetProperty("regDate").GetInt32(),
                Fuel = jsonElement.GetProperty("car").GetProperty("fuel").GetString(),
                Gearbox = jsonElement.GetProperty("car").GetProperty("gearbox").GetString(),
                CreatedAt = DateTime.Parse(jsonElement.GetProperty("listTime").GetString())
            };
        }
        catch (Exception ex)
        {
            // Handle parsing errors appropriately
            Console.WriteLine($"Error mapping ad: {ex.Message}");
            return null;
        }
    }
    
}
