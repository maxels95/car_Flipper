using CarFlipper.API.Data;
using CarFlipper.API.Models;
using Microsoft.EntityFrameworkCore;

public class MarketPriceService : IMarketPriceService
{
    private readonly AppDbContext _context;

    public MarketPriceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MarketPrice> UpdateOrCreateMarketPriceAsync(Ad ad)
    {
        var yearRange = GetYearRange(ad.ModelYear);
        var milageRange = GetMilageRange(ad.Milage);

        if (yearRange == default || milageRange == default)
            throw new Exception("Ingen lämplig intervall hittades");

        var existing = await _context.MarketPrices.FirstOrDefaultAsync(mp =>
            mp.Make == ad.Make &&
            mp.Model == ad.Model &&
            mp.Fuel == ad.Fuel &&
            mp.ModelYearFrom == yearRange.Start &&
            mp.ModelYearTo == yearRange.End &&
            mp.MilageFrom == milageRange.Start &&
            mp.MilageTo == milageRange.End
        );

        if (existing != null && (ad.Price + 10000) < existing.EstimatedPrice)
        {
            existing.EstimatedPrice = ((existing.EstimatedPrice * existing.SampleSize) + ad.Price) / (existing.SampleSize + 1);
            existing.SampleSize++;
            existing.UpdatedAt = DateTime.Now.ToUniversalTime();
        }
        else
        {
            existing = new MarketPrice
            {
                Make = ad.Make,
                Model = ad.Model,
                Fuel = ad.Fuel,
                Gearbox = ad.Gearbox,
                ModelYearFrom = yearRange.Start,
                ModelYearTo = yearRange.End,
                MilageFrom = milageRange.Start,
                MilageTo = milageRange.End,
                EstimatedPrice = ad.Price,
                SampleSize = 1
            };

            _context.MarketPrices.Add(existing);
        }

        await _context.SaveChangesAsync();
        return existing;
    }


    private (int Start, int End) GetYearRange(int year)
    {
        return YearRanges.FirstOrDefault(r => year >= r.Start && year <= r.End);
    }

    private (int Start, int End) GetMilageRange(int milage)
    {
        return MilageRanges.FirstOrDefault(r => milage >= r.Start && milage <= r.End);
    }

    private static readonly List<(int Start, int End)> YearRanges = new()
    {
        (1990, 1991),
        (1992, 1993),
        (1994, 1995),
        (1996, 1997),
        (1998, 1999),
        (2000, 2001),
        (2002, 2003),
        (2004, 2005),
        (2006, 2007),
        (2008, 2009),
        (2010, 2011),
        (2012, 2013),
        (2014, 2015),
        (2016, 2017),
        (2018, 2019),
        (2020, 2021),
        (2022, 2023),
        (2024, 2025),
    };

private static readonly List<(int Start, int End)> MilageRanges = new()
{
    (0, 2999),
    (3000, 5999),
    (6000, 8999),
    (9000, 11999),
    (12000, 14999),
    (15000, 17999),
    (18000, 20999),
    (21000, 23999),
    (24000, 26999),
    (27000, 29999),
    (30000, 32999),
    (33000, 35999),
    (36000, 38999),
    (39000, 41999),
    (42000, int.MaxValue)
};

}
