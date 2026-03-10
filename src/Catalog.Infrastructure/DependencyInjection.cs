using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Messaging;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("Catalog")));

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IGameLicenseRepository, GameLicenseRepository>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<PaymentProcessedConsumer>();
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"], "/", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"] ?? "guest");
                    h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                });
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
