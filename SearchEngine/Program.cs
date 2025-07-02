using Microsoft.Extensions.Options;
using Nest;
using SearchEngine.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.Configure<ElasticSearchSettings>(
    builder.Configuration.GetSection("Elastic")
);

builder.Services.AddSingleton<IElasticClient>(serviceProvider =>
{
    var config = serviceProvider.GetRequiredService<IOptions<ElasticSearchSettings>>().Value;

    var connectionSettings = new ConnectionSettings(new Uri(config.Uri))
        .DefaultIndex(config.DefaultIndex)
        .BasicAuthentication(config.Username, config.Password)
        .EnableDebugMode()
        .ThrowExceptions();

    return new ElasticClient(connectionSettings);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
