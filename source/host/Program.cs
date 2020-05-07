using System;
using System.Net;

using ElectronNET.API;

using EventStore.ClientAPI.Embedded;
using EventStore.Common.Options;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace host {
    public class Program {
        public static void Main(string[] args) {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .CreateLogger();


            var nodeBuilder = EmbeddedVNodeBuilder
                .AsSingleNode()
                .StartStandardProjections()
                .RunProjections(ProjectionType.All)
                .RunInMemory()
                .WithInternalHttpOn(new IPEndPoint(IPAddress.Loopback, 1112))
                .WithExternalHttpOn(new IPEndPoint(IPAddress.Loopback, 1113))
                .WithExternalTcpOn(new IPEndPoint(IPAddress.Loopback, 1114))
                .WithInternalTcpOn(new IPEndPoint(IPAddress.Loopback, 1115))
                .DisableDnsDiscovery()
                .WithGossipSeeds(new IPEndPoint[] {
                    new IPEndPoint(IPAddress.Loopback, 2112),
                        new IPEndPoint(IPAddress.Loopback, 3112)
                });
            var _node = nodeBuilder.Build();
            _node.StartAsync(true).Wait(TimeSpan.FromSeconds(30));

            Log.Information("Hello world!");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder.UseElectron(args);
                webBuilder.UseStartup<Startup>();
            });
    }
}