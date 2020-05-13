using ElectronNET.API;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace Host {
    public class Program {
        public static void Main(string[] args) {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .ConfigureServices(services => {
                services.AddHostedService<Services.EmbeddedEventStoreService>();
            })
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder.UseElectron(args);
                webBuilder.UseStartup<Startup>();
            });
    }
}