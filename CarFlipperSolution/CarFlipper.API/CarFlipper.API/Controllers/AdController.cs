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
        private readonly IAdMappingService _mappingService;

        public CarController(AppDbContext context, IAdMappingService mappingService)
        {
            _context = context;
            _mappingService = mappingService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCars([FromBody] List<AdDTO> ads)
        {
            if (ads == null || ads.Count == 0)
                return BadRequest("No ads received.");

            var existingIds = await _context.Ads
                .Where(a => ads.Select(x => x.AdId).Contains(a.AdId))
                .Select(a => a.AdId)
                .ToListAsync();

            var newAdsDTO = ads.Where(a => !existingIds.Contains(a.AdId)).ToList();

            // Kör alla asynkrona mappningar
            var mappedAds = await Task.WhenAll(newAdsDTO.Select(dto => _mappingService.MapToAd(dto)));

            // Filtrera bort null
            var newAds = mappedAds
                .Where(ad => ad != null)
                .ToList()!; // "!" för att kompilatorn inte ska klaga på null trots filtreringen

            if (newAds.Count > 0)
            {
                await _context.Ads.AddRangeAsync(newAds);
                await _context.SaveChangesAsync();
            }


            return Ok(new
            {
                Added = newAds.Count,
                Skipped = ads.Count - newAds.Count,
                Message = $"{newAds.Count} cars added, {ads.Count - newAds.Count} skipped as duplicates or invalid."
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
    }
}
