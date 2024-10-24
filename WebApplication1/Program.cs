using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Json;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.File(
        new JsonFormatter(renderMessage: true, formatProvider: CultureInfo.InvariantCulture),
        "logs/log.jsonl",
        encoding: Encoding.UTF8)
    .CreateBootstrapLogger();
var builder = WebApplication.CreateBuilder(args);

try
{
    using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
    Log.Logger.Debug("Opening store");
    store.Open(OpenFlags.ReadOnly); // <---------------------- error should occur here
    Log.Logger.Debug("Store opened");
    var certs = store.Certificates;
    foreach (var cert in certs)
    {
        Log.Logger.Information(
            "Subject: {Subject}, Thumbprint: {Thumbprint}, FriendlyName: {FriendlyName}, Issuer: {Issuer}",
            cert.Subject, cert.Thumbprint, cert.FriendlyName, cert.Issuer);
    }
}

catch (Exception e)
{
    Log.Logger.Fatal(e, "Error has occurred");
    throw;
}


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSerilog(Log.Logger);

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
