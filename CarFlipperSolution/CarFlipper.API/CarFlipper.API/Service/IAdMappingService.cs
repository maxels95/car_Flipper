using System.Text.Json;
using CarFlipper.API.DTO;
using CarFlipper.API.Models;

public interface IAdMappingService
{
    Task<Ad?> MapToAd(AdDTO dto);
    Task<AdDTO?> MapToAdDTO(JsonElement jsonElement);
}
