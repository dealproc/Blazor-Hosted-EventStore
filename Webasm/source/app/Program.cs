using System;
using System.Net.Http;
using System.Threading.Tasks;

using EventStore.Client;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace app {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.Configure<EventStoreClientSettings>(options => {
                options.ConnectivitySettings = new EventStoreClientConnectivitySettings { Address = new Uri("http://127.0.0.1:2113") };
            });
            builder.Services.AddSingleton(provider =>
            {
                var options = provider.GetService<IOptions<EventStoreClientSettings>>();
                return new EventStoreClient(options);
            });
            builder.Services.AddSingleton(provider =>
            {
                var options = provider.GetService<IOptions<EventStoreClientSettings>>();
                return new EventStorePersistentSubscriptionsClient(options.Value);
            });

            await builder.Build().RunAsync();
        }
    }
}