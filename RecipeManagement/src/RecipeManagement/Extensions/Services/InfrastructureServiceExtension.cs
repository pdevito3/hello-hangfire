namespace RecipeManagement.Extensions.Services;

using RecipeManagement.Databases;
using RecipeManagement.Resources;
using RecipeManagement.Services;
using Configurations;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;

public static class ServiceRegistration
{
    public static void AddInfrastructure(this IServiceCollection services, IWebHostEnvironment env, IConfiguration configuration)
    {
        // DbContext -- Do Not Delete
        var connectionString = configuration.GetConnectionStringOptions().RecipeManagement;
        if(string.IsNullOrWhiteSpace(connectionString))
        {
            // this makes local migrations easier to manage. feel free to refactor if desired.
            connectionString = env.IsDevelopment() 
                ? "Host=localhost;Port=49324;Database=dev_recipemanagement;Username=postgres;Password=postgres"
                : throw new Exception("The database connection string is not set.");
        }

        services.AddDbContext<RecipesDbContext>(options =>
            options.UseNpgsql(connectionString,
                builder => builder.MigrationsAssembly(typeof(RecipesDbContext).Assembly.FullName)));

        var hangfireOptions = new PostgreSqlStorageOptions()
        {
            SchemaName = "hangfire",
            PrepareSchemaIfNecessary = true
        };
        services.AddHangfire(hangfireConfig => hangfireConfig
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseColouredConsoleLogProvider()
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(connectionString, hangfireOptions)
        );
        services.AddHangfireServer(o =>
        {
            o.ServerName = $"RecipeManagement-{env.EnvironmentName}";
            o.Queues = new[] { "loop-queue" };
        });
        
        services.AddHostedService<MigrationHostedService<RecipesDbContext>>();

        // Auth -- Do Not Delete
    }
}
