using CarFlipper.API.Models;

public interface IEngineParser
{
    void LoadFromJson(string filePath);
    string? ParseEngine(string title, string description, string make, List<string> allowedEngines);
}