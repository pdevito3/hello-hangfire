namespace RecipeManagement.FunctionalTests;

using RecipeManagement.Resources;
using RecipeManagement.SharedTestHelpers.Utilities;
using Configurations;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Xunit;

[CollectionDefinition(nameof(TestBase))]
public class TestingWebApplicationFactoryCollection : ICollectionFixture<TestingWebApplicationFactory> { }

public class TestingWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly TestcontainerDatabase _dbContainer = DbSetup();
    private readonly RmqConfig _rmqContainer = RmqSetup();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Consts.Testing.FunctionalTestingEnvName);
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });
        
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            var functionalConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            configurationBuilder.AddConfiguration(functionalConfig);
        });

        builder.ConfigureServices(services =>
        {
            var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
            typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
            var mapperConfig = new Mapper(typeAdapterConfig);
            services.AddSingleton<IMapper>(mapperConfig);
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        Environment.SetEnvironmentVariable($"{ConnectionStringOptions.SectionName}__{ConnectionStringOptions.RecipeManagementKey}", _dbContainer.ConnectionString);
        // migrations applied in MigrationHostedService

        await _rmqContainer.Container.StartAsync();
        Environment.SetEnvironmentVariable($"{RabbitMqOptions.SectionName}__{RabbitMqOptions.HostKey}", "localhost");
        Environment.SetEnvironmentVariable($"{RabbitMqOptions.SectionName}__{RabbitMqOptions.VirtualHostKey}", "/");
        Environment.SetEnvironmentVariable($"{RabbitMqOptions.SectionName}__{RabbitMqOptions.UsernameKey}", "guest");
        Environment.SetEnvironmentVariable($"{RabbitMqOptions.SectionName}__{RabbitMqOptions.PasswordKey}", "guest");
        Environment.SetEnvironmentVariable($"{RabbitMqOptions.SectionName}__{RabbitMqOptions.PortKey}", _rmqContainer.Port.ToString());
    }

    public new async Task DisposeAsync() 
    {
        await _dbContainer.DisposeAsync();
        await _rmqContainer.Container.DisposeAsync();
    }

    private static TestcontainerDatabase DbSetup()
    {
        return new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = "db",
                Username = "postgres",
                Password = "postgres"
            })
            .WithName($"IntegrationTesting_RecipeManagement_{Guid.NewGuid()}")
            .WithImage("postgres:latest")
            .Build();
    }

    private class RmqConfig
    {
        public IContainer Container { get; set; }
        public int Port { get; set; }
    }

    private static RmqConfig RmqSetup()
    {
        var freePort = DockerUtilities.GetFreePort();
        return new RmqConfig
        {
            Container = new ContainerBuilder()
                .WithImage("masstransit/rabbitmq")
                .WithPortBinding(freePort, 5672)
                .WithName($"FunctionalTesting_RMQ_{Guid.NewGuid()}")
                .Build(),
            Port = freePort
        };
    }
}