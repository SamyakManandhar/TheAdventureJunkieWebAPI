using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Reflection;
using TheAdventureJunkieWebAPI.Contracts;
using TheAdventureJunkieWebAPI.Data;
using TheAdventureJunkieWebAPI.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

    setupAction.IncludeXmlComments(xmlCommentsFullPath);
    setupAction.AddServer(new OpenApiServer
    {
        Url = "https://taj-apim.azure-api.net" // Tell Swagger UI to send requests here
    });

});

builder.Services.AddDbContext<TheAdventureJunkieDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:TheAdventureJunkieDbContextConnection"]);
});
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IEventCacheService, EventCacheService>();
builder.Services.AddScoped<ICategoryCacheService, CategoryCacheService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
}).AddMvc();

// Add Redis Cache service
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["ConnectionStrings:RedisConnectionString"];
    options.InstanceName = "TAJ-WebAPI-Redis";
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TAJ API V1"); // Local Swagger JSON
    c.RoutePrefix = "swagger";
    c.ConfigObject = new()
    {
        ValidatorUrl = null
    };
});

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    if (path.StartsWith("/swagger") || path.StartsWith("/favicon.ico"))
    {
        await next();
        return;
    }

    // Uncomment the following lines to allow requests from localhost
    /*if (context.Request.Host.Host == "localhost")
    {
        await next();
        return;
    }*/

    var config = context.RequestServices.GetRequiredService<IConfiguration>();
    var expectedToken = config["APIM:SecretToken"];
    var receivedToken = context.Request.Headers["X-APIM-Signature"].FirstOrDefault();

    if (receivedToken == expectedToken)
    {
        await next();
        return;
    }

    context.Response.StatusCode = 401;
    context.Response.ContentType = "application/json";
    var errorResponse = new
    {
        type = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
        title = "Unauthorized, Please use APIM instance",
        status = 401,
        traceId = context.TraceIdentifier // Unique trace ID for this request
    };

    // Serialize the error response to JSON and write it to the response
    var jsonResponse = JsonConvert.SerializeObject(errorResponse);
    await context.Response.WriteAsync(jsonResponse);

});


app.UseAuthorization();

app.MapControllers();

app.Run();
