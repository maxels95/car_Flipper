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

        public CarController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddCars([FromBody] List<AdDTO> ads)
        {
            if (ads == null || ads.Count == 0)
                return BadRequest("No ads received.");

            // Hämta existerande AdId:n från databasen
            var existingIds = await _context.Ads
                .Where(a => ads.Select(x => x.AdId).Contains(a.AdId))
                .Select(a => a.AdId)
                .ToListAsync();

            // Filtrera bort annonser som redan finns
            var newAdsDTO = ads.Where(a => !existingIds.Contains(a.AdId)).ToList();
            var newAds = new List<Ad>();

            foreach (AdDTO ad in newAdsDTO)
            {
                var parser = new CarParserService();
                var (make, model) = parser.ParseMakeAndModel(ad.Title);
                if (make == null || model == null)
                {
                    (make, model) = parser.ParseMakeAndModel(ad.Description);
                }

                try
                    {
                        Ad newAd = new Ad();
                        newAd.AdId = ad.AdId;
                        newAd.Title = ad.Title;
                        newAd.Make = make;
                        newAd.Model = model;
                        newAd.Url = ad.Url;
                        newAd.Source = ad.Source;

                        if (Enum.TryParse<Region>(ad.Region, out var parsedRegion))
                        {
                            newAd.Region = parsedRegion;
                        }

                        newAd.Price = ad.Price;
                        newAd.Milage = ad.Milage;
                        newAd.ModelYear = ad.ModelYear;
                        newAd.Fuel = ad.Fuel;
                        newAd.Gearbox = ad.Gearbox;
                        newAd.CreatedAt = ad.CreatedAt;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Annons kunde ej läggas till: {ex.Message}");
                    }
            }

            if (newAds.Count > 0)
                {
                    _context.Ads.AddRange(newAds);
                    await _context.SaveChangesAsync();
                }

            return Ok(new
            {
                Added = newAds.Count,
                Skipped = ads.Count - newAds.Count,
                Message = $"{newAds.Count} cars added, {ads.Count - newAds.Count} skipped as duplicates."
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
            [FromQuery] string? region)
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

    }
}
