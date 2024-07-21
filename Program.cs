using System.Net.Http.Headers;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOutputCache(options =>
{

    options.AddBasePolicy(builder =>
        builder.Expire(TimeSpan.FromSeconds(10)));

    options.AddPolicy("ValidatingCache", builder => builder.Expire(TimeSpan.FromMinutes(30))
    .AddPolicy<ValidatingCachePolicy>());

    options.AddPolicy("WeatherCache", builder => builder
        .Expire(TimeSpan.FromMinutes(30))
        .VaryByValue((context) =>
        {
            return new KeyValuePair<string, string>("x", context.Request.Query["x"].ToString()[..5]);
        })
        .VaryByValue((context) =>
        {
            return new KeyValuePair<string, string>("y", context.Request.Query["y"].ToString()[..5]);
        })
        .SetVaryByQuery("")
        .AddPolicy<ValidatingCachePolicy>()
    );

    options.AddPolicy("Expire10min", builder =>
        builder.Expire(TimeSpan.FromMinutes(10)));

    options.AddPolicy("Expire30min", builder =>
        builder.Expire(TimeSpan.FromMinutes(30)));
});
builder.Services.AddHttpClient();
builder.Configuration.AddUserSecrets<Program>();
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseOutputCache();


app.MapGet("/findCity/{name}", async (string name) =>
{
    var clientFactory = app.Services.GetService<IHttpClientFactory>();
    var client = clientFactory!.CreateClient();

    var response = await client.GetAsync($"https://api.dataforsyningen.dk/postnumre/autocomplete?q={name}");
    if (response.IsSuccessStatusCode)
    {
        return Results.Stream(response.Content.ReadAsStream(), "application/json");
    }

    return Results.Problem("Got bad response");
}).CacheOutput("ValidatingCache");

app.MapGet("/getCityName", async (string x, string y) =>
{
    var clientFactory = app.Services.GetService<IHttpClientFactory>();
    var client = clientFactory!.CreateClient();

    var response = await client.GetAsync($"https://api.dataforsyningen.dk/postnumre/reverse?x={x}&y={y}");
    if (response.IsSuccessStatusCode)
    {
        return Results.Stream(response.Content.ReadAsStream(), "application/json");
    }

    return Results.Problem("Got bad response");

}).CacheOutput("ValidatingCache");



app.MapGet("/getForecast", async (string x, string y) =>
{
    var clientFactory = app.Services.GetService<IHttpClientFactory>();
    var client = clientFactory!.CreateClient();
    var requestTime = Uri.EscapeDataString(string.Format("{0:yyyy-MM-ddTHH:00:00Z}", DateTime.UtcNow));
    string? apikey = Environment.GetEnvironmentVariable("DMI_APIKEY");
    if (apikey == null && app.Environment.IsDevelopment())
    {
        apikey = app.Configuration.GetValue<string>("DMI:ApiKey");
    }

    if (apikey != null)
    {
        var response = await client.GetAsync($"https://dmigw.govcloud.dk/v1/forecastedr/collections/harmonie_dini_sf/position?coords=POINT%28{x}%20{y}%29&parameter-name=temperature-2m,total-precipitation,wind-speed,wind-dir,gust-wind-speed-10m,fraction-of-cloud-cover,precipitation-type,direct-solar-exposure,cloud-transmittance&datetime={requestTime}%2F..&crs=crs84&f=GeoJSON&api-key={apikey}");
        if (response.IsSuccessStatusCode)
        {
            return Results.Stream(response.Content.ReadAsStream(), "application/json");
        }

        return Results.Problem("Got bad response");
    }
    else
    {
        return Results.Problem("Missing api key");
    }

}).CacheOutput("WeatherCache");


app.Run();
