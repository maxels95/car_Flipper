using System.Text.Json;

public class CarParserService
    {
        private Dictionary<string, List<string>> _carData = new();
        private Dictionary<string, string> _modelToMake = new();

        public void Load(string jsonPath)
        {
            if (!File.Exists(jsonPath)) return;

            var json = File.ReadAllText(jsonPath);

            try
            {
                var list = JsonSerializer.Deserialize<List<CarBrandModels>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _carData = list!.ToDictionary(
                    x => x.Brand,
                    x => x.Models,
                    StringComparer.OrdinalIgnoreCase
                );

                _modelToMake = _carData
                    .SelectMany(kvp => kvp.Value.Select(model => new { Make = kvp.Key, Model = model }))
                    .ToDictionary(x => x.Model, x => x.Make, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Kunde inte ladda car-list.json: {ex.Message}");
                _carData = new();
                _modelToMake = new();
            }
        }


    public (string? Make, string? Model) ParseMakeAndModel(string title)
    {
        if (string.IsNullOrWhiteSpace(title) || _carData.Count == 0)
            return (null, null);

        var lowerTitle = title.ToLower();

        // Matcha make först
        foreach (var make in _carData.Keys)
        {
            if (lowerTitle.Contains(make.ToLower()))
            {
                foreach (var model in _carData[make])
                {
                    if (lowerTitle.Contains(model.ToLower()))
                        return (make, model);
                }
                return (make, null);
            }
        }

        // Om make inte hittades, försök hitta modell och mappa till make
        foreach (var model in _modelToMake.Keys)
        {
            if (lowerTitle.Contains(model.ToLower()))
            {
                var make = _modelToMake[model];
                return (make, model);
            }
        }

        return (null, null);
    }
}
