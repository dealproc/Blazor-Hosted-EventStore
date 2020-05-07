using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using EventStore.ClientAPI;
// using EventStore.ClientAPI.Embedded;
// using EventStore.Common.Options;
using System.Net;

namespace app
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // IEventStoreConnection _connection;
            // EventStoreConnection.Create(new Uri(""), "");

            // var nodeBuilder = EmbeddedVNodeBuilder
            //     .AsSingleNode()
            //     .StartStandardProjections()
            //     .RunProjections(ProjectionType.All)    
            //     .RunInMemory()
            //     .WithInternalHttpOn(new IPEndPoint(IPAddress.Loopback, 1112))
            //     .WithExternalHttpOn(new IPEndPoint(IPAddress.Loopback, 1113))
            //     .WithExternalTcpOn(new IPEndPoint(IPAddress.Loopback, 1114))
            //     .WithInternalTcpOn(new IPEndPoint(IPAddress.Loopback, 1115))
            //     .DisableDnsDiscovery()
            //     .WithGossipSeeds(new IPEndPoint[]
            //     {
            //         new IPEndPoint(IPAddress.Loopback, 2112),
            //         new IPEndPoint(IPAddress.Loopback, 3112)
            //     });                    
            // var _node = nodeBuilder.Build();
            // _node.StartAsync(true).Wait(TimeSpan.FromSeconds(30));
            
            // _connection = EmbeddedEventStoreConnection.Create(_node);

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
