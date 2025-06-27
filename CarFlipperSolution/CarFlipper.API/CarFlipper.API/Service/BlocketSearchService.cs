using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CarFlipper.API.Enums;
using CarFlipper.API.Models;
using CarFlipper.API.Helpers;

public class BlocketSearchService
{
    private readonly HttpClient _httpClient;
    private readonly BlocketAuthService _authService;
    private readonly BlocketFilterService _filterService;

    public BlocketSearchService(HttpClient httpClient, BlocketAuthService authService, BlocketFilterService filterService)
    {
        _httpClient = httpClient;
        _authService = authService;
        _filterService = filterService;
    }

    public async Task<List<Ad>> SearchSimilarAdsAsync(MarketPrice mp)
    {
        var token = await _authService.GetTokenAsync();
        var filter = _filterService.ConvertMarketPriceToFilter(mp);
        var query = _filterService.BuildUrlQueryFromFilter(filter);
        var url = $"https://api.blocket.se/motor-search-service/v2/search/car?filter={query}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.UserAgent.ParseAdd("Mozilla/5.0");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var ads = new List<Ad>();

        foreach (var item in doc.RootElement.GetProperty("cars").EnumerateArray())
        {

            // Rule out leasing ads
            var billingType = item.GetProperty("price").GetProperty("billingPeriod").GetString();

            if (billingType == "single")
            {
                var car = item.GetProperty("car");
                var regionString = car.GetProperty("location").GetProperty("region").GetString();

                var dealId = int.Parse(item.GetProperty("dealId").GetString());
                var title = item.GetSafeString("heading");
                var description = item.GetSafeString("description");
                var adUrl = item.GetSafeString("link");
                var fuel = car.GetSafeString("fuel");
                var gearbox = car.GetSafeString("gearbox");
                var modelYear = car.GetSafeInt("regDate");
                var milage = car.GetSafeInt("mileage");
                var createdAt = item.GetSafeDateTime("listTime");
                var price = item.GetSafePrice();

                if (title == null || url == null || fuel == null ||
                        gearbox == null || modelYear == null ||
                        milage == null || createdAt == null ||
                        price == null)
                {
                    Console.WriteLine($"⚠️ Skipping ad {dealId} – saknar nödvändiga fält.");
                    continue;
                }

                var ad = new Ad
                {
                    AdId = dealId,
                    Title = title,
                    Description = description ?? "",
                    Url = url,
                    Make = mp.Make,
                    Model = mp.Model,
                    Fuel = fuel,
                    Gearbox = gearbox,
                    ModelYear = modelYear.Value,
                    Milage = milage.Value,
                    Price = price.Value,
                    CreatedAt = createdAt.Value,
                    Source = "Blocket"
                };

                if (Enum.TryParse<Region>(regionString, ignoreCase: true, out var regionEnum))
                {
                    ad.RegionId = (int)regionEnum;
                }
                else
                {
                    Console.WriteLine($"Okänd region: '{regionString}'");
                }

                ads.Add(ad);
            }

        }

        return ads;
    }
}
