using LaciSynchroni.Services.Discord;
using LaciSynchroni.Shared.Data;
using LaciSynchroni.Shared.Metrics;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using LaciSynchroni.Shared.Utils;
using LaciSynchroni.Shared.Services;
using StackExchange.Redis;
using LaciSynchroni.Shared.Utils.Configuration;

namespace LaciSynchroni.Services;

public class Startup
{
    public Startup(Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var config = app.ApplicationServices.GetRequiredService<IConfigurationService<LaciConfigurationBase>>();

        var metricServer = new KestrelMetricServer(config.GetValueOrDefault<int>(nameof(LaciConfigurationBase.MetricsPort), 4982));
        metricServer.Start();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var config = Configuration.GetSection("LaciSynchroni");

        services.AddDbContextPool<LaciDbContext>(options =>
        {
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), builder =>
            {
                builder.MigrationsHistoryTable("_efmigrationshistory", "public");
            }).UseSnakeCaseNamingConvention();
            options.EnableThreadSafetyChecks(false);
        }, Configuration.GetValue(nameof(LaciConfigurationBase.DbContextPoolSize), 1024));
        services.AddDbContextFactory<LaciDbContext>(options =>
        {
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), builder =>
            {
                builder.MigrationsHistoryTable("_efmigrationshistory", "public");
                builder.MigrationsAssembly("LaciSynchroni.Shared");
            }).UseSnakeCaseNamingConvention();
            options.EnableThreadSafetyChecks(false);
        });

        services.AddSingleton(m => new LaciMetrics(m.GetService<ILogger<LaciMetrics>>(), new List<string> { },
        new List<string> { }));

        var redis = config.GetValue(nameof(ServerConfiguration.RedisConnectionString), string.Empty);
        var options = ConfigurationOptions.Parse(redis);
        options.ClientName = "Laci";
        options.ChannelPrefix = "UserData";
        ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(options);
        services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);

        services.Configure<ServicesConfiguration>(Configuration.GetRequiredSection("LaciSynchroni"));
        services.Configure<ServerConfiguration>(Configuration.GetRequiredSection("LaciSynchroni"));
        services.Configure<LaciConfigurationBase>(Configuration.GetRequiredSection("LaciSynchroni"));
        services.AddSingleton(Configuration);
        services.AddSingleton<ServerTokenGenerator>();
        services.AddSingleton<DiscordBotServices>();
        services.AddHostedService<DiscordBot>();
        services.AddSingleton<IConfigurationService<ServicesConfiguration>, LaciConfigurationServiceServer<ServicesConfiguration>>();
        services.AddSingleton<IConfigurationService<ServerConfiguration>, LaciConfigurationServiceClient<ServerConfiguration>>();
        services.AddSingleton<IConfigurationService<LaciConfigurationBase>, LaciConfigurationServiceClient<LaciConfigurationBase>>();

        services.AddHostedService(p => (LaciConfigurationServiceClient<LaciConfigurationBase>)p.GetService<IConfigurationService<LaciConfigurationBase>>());
        services.AddHostedService(p => (LaciConfigurationServiceClient<ServerConfiguration>)p.GetService<IConfigurationService<ServerConfiguration>>());
    }
}