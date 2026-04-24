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

/* TODO Use Option Pattern and Clean up Program.cs*/

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

    setupAction.IncludeXmlComments(xmlCommentsFullPath);

    if (!builder.Environment.IsDevelopment())
    {
        setupAction.AddServer(new OpenApiServer
        {
            Url = "https://taj-apim.azure-api.net"
        });
    }
});

builder.Services.AddDbContext<TheAdventureJunkieDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:TheAdventureJunkieDbContextConnection"]);
});
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IEventCacheService, EventCacheService>();
builder.Services.AddScoped<ICategoryCacheService, CategoryCacheService>();

builder.Services.AddAutoMapper(cfg => { /* custom config if needed */ }, typeof(TheAdventureJunkieWebAPI.Profiles.CategoryProfile).Assembly);

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
    var path = context.Request.Path.Value ?? string.Empty;

    if (path.StartsWith("/swagger") || path.StartsWith("/favicon.ico") || app.Environment.IsDevelopment())
    {
        await next();
        return;
    }

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
        traceId = context.TraceIdentifier
    };

    await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
});


app.UseAuthorization();

app.MapControllers();

app.Run();
