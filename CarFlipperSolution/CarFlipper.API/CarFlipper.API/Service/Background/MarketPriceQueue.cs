using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public interface IMarketPriceQueue
{
    void Enqueue(MarketPrice mp);
}

public class MarketPriceQueue : BackgroundService, IMarketPriceQueue
{
    private readonly Channel<MarketPrice> _queue = Channel.CreateUnbounded<MarketPrice>();
    private readonly IServiceProvider _serviceProvider;

    private static readonly List<(int Start, int End)> YearRanges = new()
    {
        (1990, 1991), (1992, 1993), (1994, 1995), (1996, 1997), (1998, 1999),
        (2000, 2001), (2002, 2003), (2004, 2005), (2006, 2007), (2008, 2009),
        (2010, 2011), (2012, 2013), (2014, 2015), (2016, 2017), (2018, 2019),
        (2020, 2021), (2022, 2023), (2024, 2025)
    };

    private static readonly List<(int Start, int End)> MilageRanges = new()
    {
        (0, 2999), (3000, 5999), (6000, 8999), (9000, 11999), (12000, 14999),
        (15000, 17999), (18000, 20999), (21000, 23999), (24000, 26999), (27000, 29999),
        (30000, 32999), (33000, 35999), (36000, 38999), (39000, 41999), (42000, int.MaxValue)
    };

    public MarketPriceQueue(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Enqueue(MarketPrice mp)
    {
        _queue.Writer.TryWrite(mp);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var mp in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            using var scope = _serviceProvider.CreateScope();
            var searchService = scope.ServiceProvider.GetRequiredService<BlocketSearchService>();
            var importService = scope.ServiceProvider.GetRequiredService<AdImportService>();
            var filterService = scope.ServiceProvider.GetRequiredService<BlocketFilterService>();
            var engineParser = scope.ServiceProvider.GetRequiredService<IEngineParser>();
            var adMappingService = scope.ServiceProvider.GetRequiredService<IAdMappingService>();

            var filters = GenerateFiltersFromMarketPrice(mp);

            foreach (var filter in filters)
            {
                try
                {
                    var query = filterService.BuildUrlQueryFromFilter(filter);
                    var ads = await searchService.SearchSimilarAdsAsync(mp);

                    foreach (var ad in ads)
                    {
                        if (string.IsNullOrWhiteSpace(ad.Engine))
                        {
                            var allowedEngines = adMappingService.GetRelevantEnginesForAd(ad);
                            ad.Engine = engineParser.ParseEngine(ad.Title, ad.Description, ad.Make, allowedEngines);
                        }
                    }

                    await importService.ImportAdsAsync(ads);

                    await Task.Delay(Random.Shared.Next(800, 2000), stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fel i MarketPriceQueue: {ex.Message}");
                }
            }
        }
    }

    private IEnumerable<BlocketCarFilter> GenerateFiltersFromMarketPrice(MarketPrice mp)
    {
        foreach (var year in YearRanges)
        {
            foreach (var milage in MilageRanges)
            {
                yield return new BlocketCarFilter
                {
                    Make = mp.Make,
                    Model = mp.Model,
                    YearMin = year.Start,
                    YearMax = year.End,
                    MilageMin = milage.Start,
                    MilageMax = milage.End,
                    Fuel = mp.Fuel,
                    Gearbox = mp.Gearbox
                };
            }
        }
    }
}