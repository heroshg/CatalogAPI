using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Catalog.Infrastructure.Persistence.DynamoDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Catalog.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        var publicKeyB64 = configuration["Jwt:RsaPublicKey"]
            ?? throw new InvalidOperationException("Jwt:RsaPublicKey is missing.");

        var rsa = RSA.Create();
        rsa.ImportFromPem(Encoding.UTF8.GetString(Convert.FromBase64String(publicKeyB64)));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = configuration["Jwt:Issuer"],
                    ValidAudience            = configuration["Jwt:Audience"],
                    IssuerSigningKey         = new RsaSecurityKey(rsa) { KeyId = "fcg-rsa-1" },
                    ValidAlgorithms          = new[] { SecurityAlgorithms.RsaSha256 }
                };
            });

        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddControllers();
        services.AddHealthChecks();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog API", Version = "v1" });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name        = "Authorization",
                In          = ParameterLocation.Header,
                Type        = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                Description = "Enter: Bearer {token}"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog API v1"));
        }
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health");
        return app;
    }

    public static async Task EnsureDynamoDbTablesAsync(this WebApplication app)
    {
        using var scope      = app.Services.CreateScope();
        var bootstrapper     = scope.ServiceProvider.GetRequiredService<DynamoDbBootstrapper>();
        var logger           = scope.ServiceProvider.GetRequiredService<ILogger<DynamoDbBootstrapper>>();
        var cts              = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        try
        {
            await bootstrapper.EnsureTablesExistAsync(cts.Token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao garantir tabelas DynamoDB no startup.");
            throw;
        }
    }
}
