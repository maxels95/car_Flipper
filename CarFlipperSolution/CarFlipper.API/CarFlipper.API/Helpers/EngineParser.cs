using System.Text.Json;

public class EngineData
{
    public List<string> EngineSizes { get; set; } = new();
    public List<string> EngineCodes { get; set; } = new();
}

public static class EngineParser
{
    private static EngineData? _engineData;

    public static void LoadFromJson(string filePath)
    {
        var json = File.ReadAllText(filePath);
        _engineData = JsonSerializer.Deserialize<EngineData>(json);
    }

    public static string? ParseEngine(string title)
    {
        if (string.IsNullOrEmpty(title) || _engineData == null)
            return null;

        var titleLower = title.ToLowerInvariant();

        foreach (var size in _engineData.EngineSizes)
        {
            if (titleLower.Contains(size.ToLowerInvariant()) || titleLower.Contains(size.Replace(".", ",")))
                return size;
        }

        foreach (var code in _engineData.EngineCodes)
        {
            if (titleLower.Contains(code.ToLowerInvariant()))
                return code.ToUpper(); // För konsekvens
        }

        return null;
    }
}
