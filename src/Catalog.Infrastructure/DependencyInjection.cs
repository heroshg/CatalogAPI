using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Catalog.Application.Consumers;
using Catalog.Application.Sagas;
using Catalog.Application.Sagas.Activities;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.DynamoDB;
using Catalog.Infrastructure.Persistence.DynamoDB.Repositories;
using Catalog.Infrastructure.Persistence.DynamoDB.Sagas;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());

        services.AddAWSService<IAmazonDynamoDB>();

        services.AddScoped<IDynamoDBContext>(sp =>
            new DynamoDBContext(
                sp.GetRequiredService<IAmazonDynamoDB>(),
                new DynamoDBContextConfig
                {
                    RetrieveDateTimeInUtc = true,
                    SkipVersionCheck = true,
                    ConsistentRead = false
                }));

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IGameLicenseRepository, GameLicenseRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<DynamoDbBootstrapper>();

        services.AddScoped<ApproveOrderActivity>();
        services.AddScoped<CancelOrderActivity>();

        services.AddScoped<DynamoDbOrderSagaRepositoryContextFactory>();

        services.AddScoped<DynamoDbOrderSagaRepository>();

        services.AddScoped<ISagaRepository<OrderSagaState>>(sp =>
            sp.GetRequiredService<DynamoDbOrderSagaRepository>());

        var redisConnection = configuration["Redis:ConnectionString"];

        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(opts =>
            {
                opts.Configuration = redisConnection;
                opts.InstanceName = "fcg-catalog:";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddMassTransit(x =>
        {
            x.DisableUsageTelemetry();

            x.AddSagaStateMachine<OrderSagaStateMachine, OrderSagaState>();

            x.AddConsumer<OrderCancelledEventConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"], "/", h =>
                {
                    h.Username(
                        configuration["RabbitMQ:Username"]
                        ?? throw new InvalidOperationException("RabbitMQ:Username is missing."));

                    h.Password(
                        configuration["RabbitMQ:Password"]
                        ?? throw new InvalidOperationException("RabbitMQ:Password is missing."));
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}