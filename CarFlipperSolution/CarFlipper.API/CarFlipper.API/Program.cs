using CarFlipper.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var parser = new CarParserService();
parser.Load("Data/car-list.json");
builder.Services.AddSingleton(parser);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });


builder.Services.AddScoped<IAdMappingService, AdMappingService>();
builder.Services.AddScoped<IMarketPriceService, MarketPriceService>();
builder.Services.AddScoped<AdImportService>();
builder.Services.AddScoped<BlocketFilterService>();
builder.Services.AddScoped<BlocketSearchService>();
builder.Services.AddHttpClient<BlocketAuthService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddSingleton<IMarketPriceQueue, MarketPriceQueue>();
builder.Services.AddHostedService(provider => (MarketPriceQueue)provider.GetRequiredService<IMarketPriceQueue>());
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
