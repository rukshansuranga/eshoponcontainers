using System.Runtime.InteropServices;
using Azure.Monitor.OpenTelemetry.Exporter;
using Basket.API.GrpcService;
using Basket.API.Repositries;
using Discount.Grpc.Protos;
using MassTransit;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

// MassTransit-RabbitMQ Configuration
builder.Services.AddMassTransit(config => {
    config.UsingRabbitMq((ctx, cfg) => {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        //cfg.UseHealthCheck(ctx);
    });
});

builder.Services.AddAutoMapper(typeof(Program));

//builder.Services.AddMassTransitHostedService();

builder.Services.
    AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>
    (o => o.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]));

builder.Services.AddScoped<DiscountGrpcService>();

builder.Services.AddStackExchangeRedisCache(optons => {
    optons.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});

// Define attributes for your application
var resourceBuilder = ResourceBuilder.CreateDefault()
    // add attributes for the name and version of the service
    .AddService(serviceName: "MyCompany.MyProduct.LogsDemoApi", serviceVersion: "1.0.0")
    // add attributes for the OpenTelemetry SDK version
    .AddTelemetrySdk()
    // add custom attributes
    .AddAttributes(new Dictionary<string, object>
    {
        ["host.name"] = Environment.MachineName,
        ["os.description"] = RuntimeInformation.OSDescription,
        ["deployment.environment"] =
            builder.Environment.EnvironmentName.ToLowerInvariant(),
    });

// builder.Services.AddOpenTelemetry()
//     .WithTracing(builder =>
//     {
//         builder.AddAspNetCoreInstrumentation();
//         builder.AddConsoleExporter();
//     });

// Read Azure Monitor connection string from configuration
var azmConnectionString = builder.Configuration["AzureMonitorConnectionString"];

builder.Logging.ClearProviders()
    .AddOpenTelemetry(loggerOptions =>
    {
        loggerOptions
            // define the resource
            .SetResourceBuilder(resourceBuilder)
            // add custom processor
            .AddProcessor(new CustomLogProcessor())
            // send logs to Azure Monitor
            .AddAzureMonitorLogExporter(options =>
                options.ConnectionString = azmConnectionString)
            // send logs to the console using exporter
            .AddConsoleExporter();

        loggerOptions.IncludeFormattedMessage = true;
        loggerOptions.IncludeScopes = true;
        loggerOptions.ParseStateValues = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
