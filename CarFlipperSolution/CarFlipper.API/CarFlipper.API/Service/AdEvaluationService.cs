using CarFlipper.API.Models;

namespace CarFlipper.API.Services
{
    public class AdEvaluationService
    {
        public AdEvaluationService()
        {

        }

        public bool IsUnderpriced(Ad ad, MarketPrice? mp, int threshold = 10000)
        {
            if (mp != null)
            {
                return ad.Price < mp.EstimatedPrice - threshold;
            }
            else
            {
                return false;
            }
        }
    }
}
