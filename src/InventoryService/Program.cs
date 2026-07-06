using InventoryService.Consumers;
using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Repositories;
using InventoryService.Services;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration["Seq:ServerUrl"] ?? "http://seq:5341")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("InventoryDatabase"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 10,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null)));

builder.Services.AddScoped<IDonorRepository, DonorRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IDonorService, DonorService>();
builder.Services.AddScoped<IStockService, StockService>();

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<OrderPlacedConsumer>();

    configurator.UsingRabbitMq((context, rabbit) =>
    {
        rabbit.Host(builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq", "/", host =>
        {
            host.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            host.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        rabbit.ConfigureEndpoints(context);
    });
});

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();

await InitializeDatabaseAsync<InventoryDbContext>(app);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

    if (!db.Donors.Any())
    {
        db.Donors.Add(new Donor
        {
            LegacyId = 1,
            Name = "Seed Donor",
            Email = "seed.donor@example.com",
            Phone = "050-0000000",
            City = "Seed City",
            Neighborhood = "Seed Neighborhood",
            Street = "Seed Street 1"
        });
        db.SaveChanges();
    }

    if (!db.ProductStocks.Any())
    {
        var donorId = db.Donors.Select(d => d.Id).First();

        db.ProductStocks.AddRange(
            new ProductStock
            {
                ProductId = 1,
                DonorId = donorId,
                QuantityAvailable = 50,
                UpdatedAtUtc = DateTime.UtcNow
            },
            new ProductStock
            {
                ProductId = 2,
                DonorId = donorId,
                QuantityAvailable = 10,
                UpdatedAtUtc = DateTime.UtcNow
            },
            new ProductStock
            {
                ProductId = 3,
                DonorId = donorId,
                QuantityAvailable = 25,
                UpdatedAtUtc = DateTime.UtcNow
            });

        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

static async Task InitializeDatabaseAsync<TContext>(WebApplication app) where TContext : DbContext
{
    const int maxAttempts = 15;

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

            await dbContext.Database.MigrateAsync();
            return;
        }
        catch (SqlException ex) when (attempt < maxAttempts)
        {
            app.Logger.LogWarning(
                ex,
                "Database startup failed for {DbContext} (attempt {Attempt}/{MaxAttempts}). Retrying...",
                typeof(TContext).Name,
                attempt,
                maxAttempts);

            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}
