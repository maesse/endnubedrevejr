using Microsoft.Extensions.ObjectPool;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder =>
        builder.Expire(TimeSpan.FromSeconds(10)));
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
    var stream = await client.GetStreamAsync($"https://api.dataforsyningen.dk/postnumre/autocomplete?q={name}");
    return Results.Stream(stream, "application/json");

}).CacheOutput("Expire30min");

app.MapGet("/getCityName", async (string x, string y) =>
{
    var clientFactory = app.Services.GetService<IHttpClientFactory>();
    var client = clientFactory!.CreateClient();
    var stream = await client.GetStreamAsync($"https://api.dataforsyningen.dk/postnumre/reverse?x={x}&y={y}");
    return Results.Stream(stream, "application/json");

}).CacheOutput("Expire30min");

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
        var stream = await client.GetStreamAsync($"https://dmigw.govcloud.dk/v1/forecastedr/collections/harmonie_dini_sf/position?coords=POINT%28{x}%20{y}%29&parameter-name=temperature-2m,total-precipitation,wind-speed,wind-dir,gust-wind-speed-10m,fraction-of-cloud-cover,precipitation-type,direct-solar-exposure,cloud-transmittance&datetime={requestTime}%2F..&crs=crs84&f=GeoJSON&api-key={apikey}");
        return Results.Stream(stream, "application/json");
    }
    else
    {
        return Results.Problem("Missing api key");
    }

}).CacheOutput("Expire30min");


app.Run();
