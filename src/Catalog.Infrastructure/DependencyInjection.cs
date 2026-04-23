using Catalog.Application.Sagas;
using Catalog.Application.Sagas.Activities;
using Catalog.Domain.Interfaces;
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
        // ── Banco de Dados ────────────────────────────────────────────────────
        services.AddDbContext<CatalogDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("Catalog")));

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IGameLicenseRepository, GameLicenseRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        // Saga activities resolvidas pelo container
        services.AddScoped<ApproveOrderActivity>();
        services.AddScoped<CancelOrderActivity>();

        // ── Cache — Redis ─────────────────────────────────────────────────────
        var redisConnection = configuration["Redis:ConnectionString"];
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(opts =>
            {
                opts.Configuration = redisConnection;
                opts.InstanceName   = "fcg-catalog:";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        // ── Mensageria — RabbitMQ ─────────────────────────────────────────────
        services.AddMassTransit(x =>
        {
            x.DisableUsageTelemetry();
            x.AddSagaStateMachine<OrderSagaStateMachine, OrderSagaState>()
                .EntityFrameworkRepository(r =>
                {
                    r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                    r.ExistingDbContext<CatalogDbContext>();
                    r.UsePostgres();
                });

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"], "/", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"] ?? throw new InvalidOperationException("RabbitMQ:Username is missing."));
                    h.Password(configuration["RabbitMQ:Password"] ?? throw new InvalidOperationException("RabbitMQ:Password is missing."));
                });
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
