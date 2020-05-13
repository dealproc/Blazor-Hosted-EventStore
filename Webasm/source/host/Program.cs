using ElectronNET.API;
using EventStore.ClientAPI.Embedded;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .CreateLogger();

            var nodeBuilder = EmbeddedVNodeBuilder.AsSingleNode()
                .OnDefaultEndpoints()
                .DisableExternalTls()
                .DisableInternalTls()
                .RunInMemory();
            var node = nodeBuilder.Build();
            node.StartAsync(true).Wait();

            Log.Information("Hello world!");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseElectron(args);
                    webBuilder.UseStartup<Startup>();
                });
    }
}