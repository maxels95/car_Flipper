using System.Text.Json;
using System.Text.Json.Serialization;
using CarFlipper.API.Data;
using CarFlipper.API.Models;

public class EngineData
{
    [JsonPropertyName("engine_sizes")]
    public List<string> EngineSizes { get; set; } = new();

    [JsonPropertyName("engine_codes")]
    public Dictionary<string, List<string>> EngineCodes { get; set; } = new();

    [JsonPropertyName("special_notations")]
    public List<string> SpecialNotations { get; set; } = new();
}

public class EngineParser : IEngineParser
{
    private EngineData? _engineData;

    public void LoadFromJson(string filePath)
    {
        try
        {
            Console.WriteLine($"🔍 Försöker läsa motorfil från: {filePath}");

            if (!File.Exists(filePath))
            {
                Console.WriteLine("❌ Filen hittades inte!");
                return;
            }

            var json = File.ReadAllText(filePath);
            _engineData = JsonSerializer.Deserialize<EngineData>(json);

            if (_engineData == null)
            {
                Console.WriteLine("❌ Deserialisering misslyckades – kontrollera JSON-format.");
            }
            else
            {
                Console.WriteLine($"✅ Motorfil laddad med {_engineData.EngineSizes.Count} storlekar.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fel vid laddning av motorfil: {ex.Message}");
        }
    }


    public string? ParseEngine(string title, string description, string make, List<string> allowedEngines)
    {
        if (_engineData == null)
            return null;

        var textSources = new[] { title ?? "", description ?? "" };
        var makeKey = NormalizeMake(make);
        var allowedSet = allowedEngines?
            .Select(e => e.ToLowerInvariant())
            .ToHashSet() ?? new HashSet<string>();

        List<string> matchedEngines = new();

        if (_engineData.EngineCodes.TryGetValue(makeKey, out var codesForMake))
        {
            foreach (var code in codesForMake)
            {
                var codeLower = code.ToLowerInvariant();

                foreach (var text in textSources)
                {
                    if (text.ToLowerInvariant().Contains(codeLower))
                    {
                        matchedEngines.Add(code.ToUpper());
                    }
                }
            }
        }

        // 1. Om vi har flera matchningar, välj en som finns i allowedEngines
        if (matchedEngines.Count > 0)
        {
            var preferred = matchedEngines.FirstOrDefault(m => allowedSet.Contains(m.ToLowerInvariant()));
            return preferred ?? matchedEngines.First(); // prioritera known, annars första matchen
        }

        // 2. Motorstorlek fallback
        foreach (var size in _engineData.EngineSizes)
        {
            foreach (var text in textSources)
            {
                if (text.Contains(size) || text.Contains(size.Replace(".", ",")))
                    return size.Replace(",", ".");
            }
        }

        return null;
    }

    private static string NormalizeMake(string make)
    {
        return make.ToLowerInvariant().Replace(" ", "_");
    }
}
