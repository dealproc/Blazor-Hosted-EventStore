using System;
using System.Net;

using ElectronNET.API;
using ElectronNET.API.Entities;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Embedded;
using EventStore.ClientAPI.SystemData;
using EventStore.Core;

using Host.Data;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Host {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        protected ClusterVNode Node { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            services.AddSingleton(ctx => {
                var log = ctx.GetService<ILogger<Startup>>();
                //RESEARCH: Should this be in a hosted service instead?
                var nodeBuilder = EmbeddedVNodeBuilder.AsSingleNode()
                    .DisableExternalTls()
                    .DisableInternalTls()
                    .WithInternalHttpOn(new IPEndPoint(IPAddress.Loopback, 1112))
                    .WithInternalTcpOn(new IPEndPoint(IPAddress.Loopback, 1113))
                    .DisableDnsDiscovery()
                    .WithGossipSeeds(new IPEndPoint[] {
                        new IPEndPoint(IPAddress.Loopback, 2112),
                            new IPEndPoint(IPAddress.Loopback, 3112)
                    })
                    .EnableTrustedAuth()
                    .RunInMemory();
                Node = nodeBuilder.Build();
                Node.StartAsync(true).Wait();

                var conn = EmbeddedEventStoreConnection.Create(Node, 
                    ConnectionSettings.Create()
                        .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"))
                        .Build());
                conn.ConnectAsync().Wait(TimeSpan.FromSeconds(30));

                return conn;
            });

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            if (HybridSupport.IsElectronActive) {
                ElectronBootstrap();
            }
        }

        public async void ElectronBootstrap() {
            var opts = new BrowserWindowOptions {
                Width = 1024,
                Height = 768,
                Show = false,
                WebPreferences = new WebPreferences {
                WebSecurity = false
                }
            };
            var browserWindow = await Electron.WindowManager.CreateWindowAsync(opts);

            await browserWindow.WebContents.Session.ClearCacheAsync();

            browserWindow.OnReadyToShow += () => {
                browserWindow.Show();
            };

            browserWindow.OnClose += () => Environment.Exit(0);
        }
    }
}