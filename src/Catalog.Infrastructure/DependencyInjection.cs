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
        services.AddDbContext<CatalogDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("Catalog")));

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IGameLicenseRepository, GameLicenseRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        // Saga activities resolvidas pelo container
        services.AddScoped<ApproveOrderActivity>();
        services.AddScoped<CancelOrderActivity>();

        services.AddMassTransit(x =>
        {
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
                    h.Username(configuration["RabbitMQ:Username"] ?? "guest");
                    h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                });
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
