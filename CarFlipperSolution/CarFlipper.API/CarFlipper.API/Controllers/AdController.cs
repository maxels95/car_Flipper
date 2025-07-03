using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarFlipper.API.Data;
using CarFlipper.API.Models;
using CarFlipper.API.Enums;
using CarFlipper.API.DTO;

namespace CarFlipper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AdImportService _adImportService;
        private readonly IMarketPriceQueue _marketPriceQueue;
        private readonly MailService _mailService;

        public CarController(AppDbContext context, AdImportService adImportService,
        IMarketPriceQueue marketPriceQueue, MailService mailService)
        {
            _context = context;
            _adImportService = adImportService;
            _marketPriceQueue = marketPriceQueue;
            _mailService = mailService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCars([FromBody] List<AdDTO> ads)
        {
            var (added, skipped) = await _adImportService.ImportAdsAsync(ads);

            if (added.Count > 0)
            {
                foreach (var ad in added)
                {

                    try
                    {
                        if (ad.IsUnderpriced)
                        {
                            _mailService.SendUndervaluedAdAlertAsync(ad);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fel vid försök att skicka mail: {ex.Message}");
                    }

                    try
                    {
                        var marketPrice = await _context.MarketPrices.FirstOrDefaultAsync(mp =>
                        mp.Id == ad.MarketPriceId);
                        _marketPriceQueue.Enqueue(marketPrice);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fel i tiläggning av bakgrundshämtning: {ex.Message}");
                    }
                }
            }

            return Ok(new
            {
                Added = added,
                Skipped = skipped,
                Message = $"{added} cars added, {skipped} skipped as duplicates or invalid."
            });
        }


        [HttpGet]
        public async Task<IActionResult> GetCars(
            [FromQuery] string? make,
            [FromQuery] string? model,
            [FromQuery] int? startYear,
            [FromQuery] int? endYear,
            [FromQuery] int? minMilage,
            [FromQuery] int? maxMilage,
            [FromQuery] int? minPrice,
            [FromQuery] int? maxPrice,
            [FromQuery] string? gearbox,
            [FromQuery] string? fuel,
            [FromQuery] string? region,
            [FromQuery] bool? IsUnderpriced)
        {
            var query = _context.Ads.AsQueryable();

            if (!string.IsNullOrEmpty(make)) query = query.Where(c => c.Make == make);
            if (!string.IsNullOrEmpty(model)) query = query.Where(c => c.Model == model);
            if (startYear.HasValue) query = query.Where(c => c.ModelYear >= startYear);
            if (endYear.HasValue) query = query.Where(c => c.ModelYear <= endYear);
            if (minMilage.HasValue) query = query.Where(c => c.Milage >= minMilage);
            if (maxMilage.HasValue) query = query.Where(c => c.Milage <= maxMilage);
            if (minMilage.HasValue) query = query.Where(c => c.Price >= minPrice);
            if (maxMilage.HasValue) query = query.Where(c => c.Price <= maxPrice);
            if (!string.IsNullOrEmpty(gearbox)) query = query.Where(c => c.Gearbox == gearbox);
            if (!string.IsNullOrEmpty(fuel)) query = query.Where(c => c.Fuel == fuel);
            if (!string.IsNullOrEmpty(region) && Enum.TryParse<Region>(region, out var parsedRegion)) query = query.Where(c => c.RegionId == (int)parsedRegion);
            if (!IsUnderpriced.HasValue) query = query.Where(c => c.IsUnderpriced == IsUnderpriced);

            var result = await query.ToListAsync();
            return Ok(result);
        }

        [HttpPut("{adId}")]
        public async Task<IActionResult> UpdateAd(int adId, [FromBody] Ad update)
        {
            var ad = await _context.Ads.FirstOrDefaultAsync(a => a.AdId == adId);

            if (ad == null)
            {
                return NotFound(new { Message = $"No ad found with AdId: {adId}" });
            }

            // Uppdatera fält – du kan lägga till fler här
            ad.Deprecated = update.Deprecated;
            ad.RemovedAt = update.RemovedAt;
            ad.IsUnderpriced = update.IsUnderpriced;

            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Ad with AdId {adId} updated." });
        }
        
        [HttpDelete("reset-database")]
        public async Task<IActionResult> ResetDatabase()
        {
            _context.Ads.RemoveRange(_context.Ads);
            _context.PriceHistories.RemoveRange(_context.PriceHistories);
            _context.MarketPrices.RemoveRange(_context.MarketPrices);

            await _context.SaveChangesAsync();

            return Ok(new { Message = "All data deleted." });
        }
    }
}
