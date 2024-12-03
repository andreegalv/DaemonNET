using DaemonNET;
using DaemonNET.Configuration;
using NLog.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// ------------------------------------ Log Configuration ----------------------------------------------------
{
    builder.Logging.ClearProviders();
    builder.Logging.AddNLog();
}

builder.Services.AddHttpClient();

// ------------------------------------ Daemon Configuration -------------------------------------------------
{
    builder.Services.AddHostedService<DaemonBackgroundService>();
    builder.Services.AddSingleton<IDaemonConfiguration>((_) => builder.Configuration
                            .AddJsonFile("daemon.json")
                            .Build()
                            .Get<DaemonConfiguration>() ?? throw new FileNotFoundException("Daemon config not found"));

    builder.Services.AddWindowsService(options =>
    {
        using (var prov = builder.Services.BuildServiceProvider())
        {
            var config = prov.GetRequiredService<IDaemonConfiguration>();
            options.ServiceName = config.Name;
        }
    });

    var host = builder.Build();
    host.Run();
}