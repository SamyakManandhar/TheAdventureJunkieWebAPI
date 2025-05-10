using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
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
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    if (path.StartsWith("/swagger") || path.StartsWith("/favicon.ico"))
    {
        await next();
        return;
    }

    if (context.Request.Host.Host == "localhost")
    {
        await next();
        return;
    }

    var config = context.RequestServices.GetRequiredService<IConfiguration>();

    var origin = context.Request.Headers["Origin"].ToString();
    var expectedOrigin = config["APIM:ExpectedOrigin"];
    if (!string.IsNullOrWhiteSpace(expectedOrigin) && origin == expectedOrigin)
    {
        await next();
        return;
    }

    var expectedToken = config["APIM:SecretToken"];
    var receivedToken = context.Request.Headers["X-APIM-Signature"].FirstOrDefault();

    if (receivedToken == expectedToken)
    {
        await next();
        return;
    }

    context.Response.StatusCode = 401;
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync("{\"error\": \"Unauthorized. Use API Management.\"}");
});


app.UseAuthorization();

app.MapControllers();

app.Run();
