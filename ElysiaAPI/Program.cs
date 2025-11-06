using System.Text;
using Asp.Versioning;
using ElysiaAPI.Application.Services;
using ElysiaAPI.Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDB"))
);

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddControllers();
builder.Services.AddRouting(o => o.LowercaseUrls = true);

builder.Services
    .AddApiVersioning(o =>
    {
        o.DefaultApiVersion = new ApiVersion(1, 0);
        o.AssumeDefaultVersionWhenUnspecified = true;
        o.ReportApiVersions = true;
    })
    .AddApiExplorer(o =>
    {
        o.GroupNameFormat = "'v'VVV";  
        o.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
    .AddDbContextCheck<AppDbContext>("db", tags: new[] { "ready" });

var jwtKey =
    builder.Configuration["Jwt:Key"]
    ?? Environment.GetEnvironmentVariable("JWT__KEY")
    ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

var jwtIssuer   = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("JWT Key não configurada.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SupportNonNullableReferenceTypes();

    var security = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    opt.AddSecurityDefinition("Bearer", security);
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement { { security, Array.Empty<string>() } });

    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Elysia API", Version = "v1", Description = "API Elysia - v1" });
    opt.SwaggerDoc("v2", new OpenApiInfo { Title = "Elysia API", Version = "v2", Description = "API Elysia - v2" });

    var xml = Path.Combine(AppContext.BaseDirectory, "ElysiaAPI.xml");
    if (File.Exists(xml))
        opt.IncludeXmlComments(xml, includeControllerXmlComments: true);
});

var app = builder.Build();

app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
    c.PreSerializeFilters.Add((doc, req) =>
    {
        var scheme   = req.Headers["X-Forwarded-Proto"].FirstOrDefault() ?? req.Scheme;
        var host     = req.Headers["X-Forwarded-Host"].FirstOrDefault()  ?? req.Host.Value;
        var basePath = req.PathBase.HasValue ? req.PathBase.Value : string.Empty;
        doc.Servers = new List<OpenApiServer> { new() { Url = $"{scheme}://{host}{basePath}" } };
    });
});

app.UseSwaggerUI(opt =>
{
    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Elysia API v1");
    opt.SwaggerEndpoint("/swagger/v2/swagger.json", "Elysia API v2");
    opt.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.Run();

public partial class Program { }
