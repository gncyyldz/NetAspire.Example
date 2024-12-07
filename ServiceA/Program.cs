using MassTransit;
using Microsoft.EntityFrameworkCore;
using ServiceA.Consumers;
using ServiceA.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRedisClient(connectionName: "redis");

builder.Services.AddHttpClient();

//builder.Services.AddAuthentication()
//    .AddKeycloakJwtBearer("keycloak", realm: "example-realm", options =>
//    {
//        options.Audience = "aspire.api";
//    });

//builder.AddSqlServerDbContext<ExampleDBContext>(connectionName: "exampleDB");
builder.Services.AddDbContext<ExampleDBContext>(options => options.UseSqlServer("exampleDB"));

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<ServiceBSentMessageConsumer>();
    configurator.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration.GetConnectionString("RabbitMQ"));
        _configure.ReceiveEndpoint("servicea-message-queue", e => e.ConfigureConsumer<ServiceBSentMessageConsumer>(context));
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", async (HttpClient httpClient) =>
{
    //var response = await httpClient.GetAsync("https://localhost:7257/api/data");
    var response = await httpClient.GetAsync("https://serviceb/api/data");
    response.EnsureSuccessStatusCode();
    var data = await response.Content.ReadAsStringAsync();
    return Results.Ok(data);
});

app.MapGet("cache-the-data", (IConnectionMultiplexer connectionMultiplexer) =>
{
    var db = connectionMultiplexer.GetDatabase(1);
    db.StringSet("name", "gncy");
});

app.MapGet("get-the-data-fron-the-cache", (IConnectionMultiplexer connectionMultiplexer) =>
{
    var db = connectionMultiplexer.GetDatabase(1);
    var name = db.StringGet("name");
    if (!string.IsNullOrEmpty(name))
    {
        return name.ToString();
    }
    return "";
});

app.MapGet("/get-database-state", async (ExampleDBContext exampleDBContext)
    =>
{
    return exampleDBContext.Database.GetDbConnection().State;
});

app.Run();