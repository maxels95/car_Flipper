using System.Text.Json;

public class CarParserService
{
    private readonly Dictionary<string, List<string>> _carData;
    private readonly Dictionary<string, string> _modelToMake;

    public CarParserService(string jsonPath = "Data/car-list.json")
    {
        if (!File.Exists(jsonPath))
            throw new FileNotFoundException("Hittar inte car-list.json");

        var json = File.ReadAllText(jsonPath);

        _carData = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new();

        // Bygg en uppslagslista från modell till märke (enklare reverse lookup)
        _modelToMake = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (make, models) in _carData)
        {
            foreach (var model in models)
            {
                if (!_modelToMake.ContainsKey(model))
                    _modelToMake[model] = make;
            }
        }
    }

    public (string? Make, string? Model) ParseMakeAndModel(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return (null, null);

        title = title.ToLower();

        // Försök hitta make och model i samma körning
        foreach (var make in _carData.Keys)
        {
            if (title.Contains(make.ToLower()))
            {
                foreach (var model in _carData[make])
                {
                    if (title.Contains(model.ToLower()))
                    {
                        return (make, model);
                    }
                }

                return (make, null); // märke hittad men ingen modell
            }
        }

        // Om make inte hittades – försök hitta modell och härleda märke
        foreach (var model in _modelToMake.Keys)
        {
            if (title.Contains(model.ToLower()))
            {
                var make = _modelToMake[model];
                return (make, model);
            }
        }

        return (null, null); // ingenting hittat
    }
}
