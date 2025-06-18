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

    public AdMappingService(AppDbContext context, IMarketPriceService marketPriceService)
    {
        _parser = new CarParserService();
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
    
    
}
