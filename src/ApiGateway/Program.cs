using MassTransit;
using Microsoft.OpenApi.Models;
using Serilog;
using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration["Seq:ServerUrl"] ?? "http://seq:5341")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHttpClient("order-service", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:OrderServiceBaseUrl"] ?? "http://orderservice:8080");
});

builder.Services.AddHttpClient("catalog-service", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:CatalogServiceBaseUrl"] ?? "http://productcatalogservice:8080");
});

builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingRabbitMq((_, rabbit) =>
    {
        rabbit.Host(builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq", "/", host =>
        {
            host.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            host.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Store API Gateway",
        Version = "v1",
        Description = "Aggregated gateway surface for all Store microservices"
    });
});
builder.Services.AddHealthChecks();

var swaggerEndpoints = builder.Configuration
    .GetSection("SwaggerEndpoints")
    .Get<SwaggerEndpointConfig[]>()
    ?? [];

var app = builder.Build();

app.UseSwagger(options =>
{
    options.RouteTemplate = "gateway-swagger/{documentName}/swagger.json";
});
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/gateway-swagger/v1/swagger.json", "ApiGateway");

    foreach (var endpoint in swaggerEndpoints)
    {
        options.SwaggerEndpoint(endpoint.Url, endpoint.Name);
    }
});

app.MapGet("/api/bff/order-details/{orderId:int}", async (
    int orderId,
    IHttpClientFactory httpClientFactory,
    CancellationToken cancellationToken) =>
{
    var orderClient = httpClientFactory.CreateClient("order-service");
    var orderResponse = await orderClient.GetAsync($"/api/baskets/{orderId}", cancellationToken);

    if (!orderResponse.IsSuccessStatusCode)
    {
        return orderResponse.StatusCode == System.Net.HttpStatusCode.NotFound
            ? Results.NotFound(new { message = $"Order {orderId} was not found." })
            : Results.Problem($"Failed to fetch order {orderId} from OrderService.");
    }

    var order = await orderResponse.Content.ReadFromJsonAsync<BffOrderResponse>(cancellationToken);
    if (order is null)
    {
        return Results.Problem($"Order {orderId} payload was empty.");
    }

    var catalogClient = httpClientFactory.CreateClient("catalog-service");
    var productResponse = await catalogClient.GetAsync($"/api/products/by-legacy/{order.ProductId}", cancellationToken);

    BffProductResponse? product = null;
    if (productResponse.IsSuccessStatusCode)
    {
        product = await productResponse.Content.ReadFromJsonAsync<BffProductResponse>(cancellationToken);
    }

    var result = new
    {
        order.Id,
        order.CorrelationId,
        order.UserId,
        order.ProductId,
        order.Quantity,
        order.Status,
        order.UnitPrice,
        order.CreatedAtUtc,
        Product = product
    };

    return Results.Ok(result);
})
.WithName("GetOrderDetailsBff")
.WithTags("BFF");

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapHealthChecks("/health");

app.MapReverseProxy();

app.Run();

internal sealed record BffOrderResponse(
    int Id,
    Guid CorrelationId,
    int LegacyId,
    int UserId,
    int ProductId,
    int Quantity,
    string Status,
    decimal UnitPrice,
    DateTime CreatedAtUtc);

internal sealed record BffProductResponse(
    string Id,
    int LegacyId,
    string Name,
    int GiftNumber,
    decimal Price,
    int Stock,
    string PathImage,
    string CategoryId,
    string CategoryName,
    DateTime UpdatedAtUtc);

internal sealed class SwaggerEndpointConfig
{
    public string Name { get; init; } = string.Empty;

    public string Url { get; init; } = string.Empty;
}
