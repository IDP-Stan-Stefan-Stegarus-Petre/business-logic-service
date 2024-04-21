using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MobyLabWebProgramming.Infrastructure.Configurations;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using MobyLabWebProgramming.Infrastructure.Converters;
using Serilog;
using Serilog.Events;

namespace MobyLabWebProgramming.Infrastructure.Extensions;

public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// This extension method adds the CORS configuration to the application builder.
    /// </summary>
    public static WebApplicationBuilder AddCorsConfiguration(this WebApplicationBuilder builder)
    {
        var corsConfiguration = builder.Configuration.GetSection(nameof(CorsConfiguration)).Get<CorsConfiguration>();
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policyBuilder =>
                {
                    policyBuilder.WithOrigins(corsConfiguration.Origins) // This adds the valid origins that the browser client can have.
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        return builder;
    }

    /// <summary>
    /// This extension method adds the controllers and JSON serialization configuration to the application builder.
    /// </summary>
    public static WebApplicationBuilder AddApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer()
            .AddMvc()
            .AddJsonOptions(options => {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); // Adds a conversion by name of the enums, otherwise numbers representing the enum values are used.
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // This converts the public property names of the objects serialized to Camel case.
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; // When deserializing request the properties of the JSON are mapped ignoring the casing.
            });

        return builder;
    }

    public static WebApplicationBuilder AddAuthorizationWithSwagger(this WebApplicationBuilder builder, string application)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.SchemaFilter<SmartEnumSchemaFilter>();
            c.SwaggerDoc("v1", new() { Title = application, Version = "v1" }); // Adds the application name and version, there can be more than one version for the API.
            c.AddSecurityDefinition("Bearer", new() // This is to configure the authorization in the Swagger client so that you may test authorized routes.
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });
            c.AddSecurityRequirement(new()
            {
                {
                    new()
                    {
                        Reference = new()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return builder;
    }

    /// <summary>
    /// This extension method adds any necessary services to the application builder that need to be injected by the framework.
    /// </summary>
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<DbReadWriteServiceConfiguration>(builder.Configuration.GetSection(nameof(DbReadWriteServiceConfiguration)));

        return builder;
    }

    /// <summary>
    /// This extension method adds the advanced logging configuration to the application builder.
    /// </summary>
    public static WebApplicationBuilder UseLogger(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((_, logger) =>
        {
            logger
                .MinimumLevel.Is(LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithThreadId()
                .WriteTo.Console();
        });

        return builder;
    }
}
