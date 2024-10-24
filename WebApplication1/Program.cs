using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

try
{
    using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
    Debug.WriteLine("Opening store");
    store.Open(OpenFlags.ReadOnly); // <---------------------- error should occur here
    Debug.WriteLine("Store opened");
    var certs = store.Certificates;
    foreach (var cert in certs)
    {
        Console.WriteLine(
            $"Subject: {cert.Subject}, Thumbprint: {cert.Thumbprint}, FriendlyName: {cert.FriendlyName}, Issuer: {cert.Issuer}");
    }
}

catch (Exception e)
{
    Console.Error.WriteLine(e);
    throw;
}


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
