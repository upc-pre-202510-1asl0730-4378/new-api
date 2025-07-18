using System.Text.Json.Serialization;
using Cortex.Mediator.Commands;
using eb4395u202318323.API.Shared.Infrastructure.Interfaces.ASP.Configuration;
using eb4395u202318323.API.Shared.Infrastructure.Mediator.Cortext.Configuration;
using eb4395u202318323.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using IUnitOfWork = eb4395u202318323.API.Shared.Domain.Repositories.IUnitOfWork;
using UnitOfWork = eb4395u202318323.API.Shared.Infrastructure.Persistence.EFC.Repositories.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Configure routing and controller conventions.
/// </summary>
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Enable enum serialization as strings and case-insensitive properties
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (connectionString == null) throw new InvalidOperationException("Connection string not found");

/// <summary>
/// Configure MySQL DbContext with logging options.
/// </summary>
/// <remarks>
/// Ethan Aliaga
/// </remarks>
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
        options.UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    else if (builder.Environment.IsProduction())
        options.UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Error);
});

/// <summary>
/// Add Swagger documentation generator.
/// </summary>
/// <remarks>
/// Ethan Aliaga
/// </remarks>
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new OpenApiInfo()
        {
            Title = "ACME.LearningCenterPlatform.API",
            Version = "v1",
            Description = "ACME Learning Center Platform API",
            TermsOfService = new Uri("https://acme-learning.com/tos"),
            Contact = new OpenApiContact
            {
                Name = "ACME Studios",
                Email = "contact@acme.com"
            },
            License = new OpenApiLicense
            {
                Name = "Apache 2.0",
                Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0")
            },
        });
    options.EnableAnnotations();
});

/// <summary>
/// Configure CORS to allow any origin, method and header.
/// </summary>
/// <remarks>
/// Ethan Aliaga
/// </remarks>
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy", 
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod().AllowAnyHeader());
});


// ------------------------------
// Dependency Injection
// ------------------------------

/// <summary>
/// Dependency injection for the Shared bounded context.
/// </summary>
/// <remarks>
/// Ethan Aliaga
/// </remarks>
builder.Services.AddScoped<IUnitOfWok, UnitOfWork>();


// ------------------------------
// Mediator Configuration
// ------------------------------

/// <summary>
/// Add logging behavior to the command pipeline.
/// </summary>
/// <remarks>
/// Ethan Aliaga
/// </remarks>
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(LoggingCommandBehavior<>));

// If you later add Cortex Mediator configuration, you can enable it here.

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

// ------------------------------
// Configure the HTTP pipeline
// ------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllPolicy");
app.UseAuthorization();
app.MapControllers();
app.Run();