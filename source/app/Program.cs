using System;
using System.Net.Http;
using System.Threading.Tasks;

using EventStore.ClientAPI;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace app {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            // event store
            builder.Services.AddSingleton<IEventStoreConnection>((ctx) => {
                // var options = ctx.GetService<IOptions<ConnectionStringsConfiguration>>();
                // var config = options.Value;
                //var conn = EventStoreConnection.Create(config.EventStore, $"processing-api-{System.Environment.MachineName}");
                var conn = EventStoreConnection.Create("ConnectTo=tcp://admin:changeit@127.0.0.1:1113; HeartBeatTimeout=300000", $"blazor-client-{System.Environment.MachineName}");
                conn.ConnectAsync().Wait(); //TODO: Is there a better way than saying "Wait()"?
                return conn;
            });

            await builder.Build().RunAsync();
        }
    }
}