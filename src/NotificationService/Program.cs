using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NotificationService.Consumers;
using NotificationService.Data;
using NotificationService.Repositories;
using NotificationService.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration["Seq:ServerUrl"] ?? "http://seq:5341")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("NotificationDatabase"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 10,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null)));

builder.Services.AddScoped<IWinnerRepository, WinnerRepository>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IWinnerService, WinnerService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<OrderStatusConsumer>();

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

await InitializeDatabaseAsync<NotificationDbContext>(app);

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
