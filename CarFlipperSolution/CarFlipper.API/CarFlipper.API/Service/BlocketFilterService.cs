using System.Net;
using System.Text.Json;

public class BlocketFilterService
{
    public string BuildUrlQueryFromFilter(BlocketCarFilter filter)
    {
        var filters = new List<string>();

        void AddFilter(object obj)
        {
            var json = JsonSerializer.Serialize(obj);
            filters.Add(WebUtility.UrlEncode(json));
        }

        if (!string.IsNullOrEmpty(filter.Make))
        {
            AddFilter(new { key = "make", values = new[] { filter.Make } });
        }

        if (!string.IsNullOrEmpty(filter.Model))
        {
            AddFilter(new { key = "models", values = filter.Model });
        }

        if (filter.PriceMin.HasValue || filter.PriceMax.HasValue)
        {
            AddFilter(new { key = "price", range = new { start = filter.PriceMin?.ToString() ?? "", end = filter.PriceMax?.ToString() ?? "" } });
        }

        if (filter.MilageMin.HasValue || filter.MilageMax.HasValue)
        {
            AddFilter(new { key = "milage", range = new { start = filter.MilageMin?.ToString() ?? "", end = filter.MilageMax?.ToString() ?? "" } });
        }

        if (!string.IsNullOrEmpty(filter.Gearbox))
        {
            AddFilter(new { key = "gearbox", values = new[] { filter.Gearbox } });
        }

        if (!string.IsNullOrEmpty(filter.Fuel))
        {
            AddFilter(new { key = "fuel", values = new[] { filter.Fuel } });
        }

        if (filter.YearMin.HasValue || filter.YearMax.HasValue)
        {
            AddFilter(new { key = "modelYear", range = new { start = filter.YearMin?.ToString() ?? "", end = filter.YearMax?.ToString() ?? "" } });
        }

        return string.Join("&", filters);
    }

    public BlocketCarFilter ConvertMarketPriceToFilter(MarketPrice mp)
    {
        return new BlocketCarFilter
        {
            Make = mp.Make,
            Model = mp.Model ,
            YearMin = mp.ModelYearFrom,
            YearMax = mp.ModelYearTo,
            MilageMin = mp.MilageFrom,
            MilageMax = mp.MilageTo,
            Fuel = mp.Fuel,
            Gearbox = mp.Gearbox
        };
    }
}