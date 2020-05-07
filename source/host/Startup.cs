using System;
using System.IO;

using ElectronNET.API;
using ElectronNET.API.Entities;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace host {
    public class Startup {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) { }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                    RequestPath = ""
            });

            app.UseBlazorFrameworkFiles();

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