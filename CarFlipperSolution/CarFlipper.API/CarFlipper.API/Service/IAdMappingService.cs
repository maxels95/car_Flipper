using CarFlipper.API.DTO;
using CarFlipper.API.Models;

public interface IAdMappingService
{
    Task<Ad?> MapToAd(AdDTO dto);
}
