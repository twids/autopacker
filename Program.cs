using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

// See https://aka.ms/new-console-template for more information


Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddEnvironmentVariables(prefix: "AUTOPACKER_")
            .AddCommandLine(args);
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<DirectoryWatcher>()
                .AddScoped<Unpacker>();     //services.AddHostedService<SampleHostedService>();
    })
    .UseSerilog()
    .Build()
    .RunAsync();
