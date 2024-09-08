using Drawing;
using Drawing.Models;
using Drawing.Services;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddMemoryCache();

var app = builder.Build();


// Configure the HTTP request pipeline.
app.MapGrpcService<DrawingServiceImpl>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
MemoryCache cache = (MemoryCache)app.Services.GetRequiredService<IMemoryCache>();

lifetime.ApplicationStopping.Register(() =>
{
    var keys = cache.Keys.ToList();

    foreach (var key in keys)
    {
        try
        {
            File.Delete(cache.Get<string>(key));
        }
        catch
        { }
    }

    cache.Clear();
});

app.Run();


