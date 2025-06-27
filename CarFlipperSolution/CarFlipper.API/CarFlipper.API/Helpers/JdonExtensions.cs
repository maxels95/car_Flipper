using System.Text.Json;

namespace CarFlipper.API.Helpers
{
    public static class JsonExtensions
    {
        public static string? GetSafeString(this JsonElement obj, string propName)
        {
            if (obj.TryGetProperty(propName, out var prop) && prop.ValueKind == JsonValueKind.String)
            {
                return prop.GetString();
            }

            Console.WriteLine($"⚠️ Saknar eller ogiltigt fält (string): '{propName}'");
            return null;
        }

        public static int? GetSafeInt(this JsonElement obj, string propName)
        {
            if (obj.TryGetProperty(propName, out var prop))
            {
                if (prop.ValueKind == JsonValueKind.Number && prop.TryGetInt32(out var i))
                    return i;

                if (prop.ValueKind == JsonValueKind.String && int.TryParse(prop.GetString(), out var s))
                    return s;
            }

            Console.WriteLine($"⚠️ Saknar eller ogiltigt fält (int): '{propName}'");
            return null;
        }

        public static DateTime? GetSafeDateTime(this JsonElement obj, string propName)
        {
            if (obj.TryGetProperty(propName, out var prop) &&
                prop.ValueKind == JsonValueKind.String &&
                DateTime.TryParse(prop.GetString(), out var dt))
            {
                return dt;
            }

            Console.WriteLine($"⚠️ Saknar eller ogiltigt fält (datetime): '{propName}'");
            return null;
        }

        public static int? GetSafePrice(this JsonElement obj)
        {
            if (obj.TryGetProperty("price", out var priceElement) &&
                priceElement.TryGetProperty("amount", out var amountElement) &&
                amountElement.ValueKind == JsonValueKind.String)
            {
                var priceString = amountElement.GetString();

                if (priceString != null)
                {
                    // Ta bort mellanrum och "kr"
                    priceString = priceString.Replace(" ", "").Replace("kr", "").Replace(",", "");

                    if (int.TryParse(priceString, out var price))
                        return price;
                }
            }

            Console.WriteLine("⚠️ Kunde inte tolka pris från JSON.");
            return null;
        }
    }
}
