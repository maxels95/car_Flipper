using CarFlipper.API.Data;
using CarFlipper.API.DTO;
using CarFlipper.API.Models;
using Microsoft.EntityFrameworkCore;

public class AdImportService
{
    private readonly AppDbContext _context;
    private readonly IAdMappingService _mappingService;
    private readonly IMarketPriceService _marketPriceService;

    public AdImportService(AppDbContext context, IAdMappingService mappingService, IMarketPriceService marketPriceService)
    {
        _context = context;
        _mappingService = mappingService;
        _marketPriceService = marketPriceService;
    }

    public async Task<(List<Ad?> added, int skipped)> ImportAdsAsync(List<AdDTO> ads)
    {
        if (ads == null || ads.Count == 0)
            return (null, 0);

        var existingIds = await _context.Ads
            .Where(a => ads.Select(x => x.AdId).Contains(a.AdId))
            .Select(a => a.AdId)
            .ToListAsync();

        var newAdsDTO = ads.Where(a => !existingIds.Contains(a.AdId)).ToList();

        var mappedAds = await Task.WhenAll(newAdsDTO.Select(dto => _mappingService.MapToAd(dto)));

        var newAds = mappedAds.Where(ad => ad != null).ToList()!;

        if (newAds.Count > 0)
        {
            await _context.Ads.AddRangeAsync(newAds);
            await _context.SaveChangesAsync();
        }

        return (newAds, ads.Count - newAds.Count);
    }

    public async Task<(int added, int skipped)> ImportAdsAsync(List<Ad> ads)
    {
        if (ads == null || ads.Count == 0)
            return (0, 0);

        var existingIds = await _context.Ads
            .Where(a => ads.Select(x => x.AdId).Contains(a.AdId))
            .Select(a => a.AdId)
            .ToListAsync();

        var newAds = ads.Where(a => !existingIds.Contains(a.AdId)).ToList();

        foreach (var ad in newAds)
        {
            var marketPrice = await _marketPriceService.UpdateOrCreateMarketPriceAsync(ad);
            ad.MarketPriceId = marketPrice.Id;

            if ((ad.Price + 10000) < marketPrice.EstimatedPrice)
            {
                ad.IsUnderpriced = true;
            }
        }
        
        if (newAds.Count > 0)
        {
            await _context.Ads.AddRangeAsync(newAds);
            await _context.SaveChangesAsync();
        }



        return (newAds.Count, ads.Count - newAds.Count);
    }
}