using CarFlipper.API.Models;

public interface IMarketPriceService
{
    Task<MarketPrice> UpdateOrCreateMarketPriceAsync(Ad ad);
}
