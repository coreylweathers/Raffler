using Blazored.Modal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using raffler.Hubs;
using shared.Services;

namespace raffler
{
    public class Startup
    {
        public Startup(IConfiguration config) { }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazoredModal();

            services.AddSingleton<IRaffleService, RaffleService>();
            services.AddSingleton<IPrizeService, PrizeService>();
            services.AddSingleton<IRaffleStorageService, TwilioRaffleStorageService>();
            services.AddSingleton<IPrizeStorageService, TwilioPrizeStorageService>();

            services.AddHttpClient();

            services.AddTransient<HubConnectionBuilder>();

            services.AddCors(action =>
            {
                action.AddDefaultPolicy(policy =>
                {
                    // TODO: Refine CORS method to restrict origin to list in appsettings
                    policy.AllowAnyOrigin();
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapHub<RaffleHub>("/rafflehub");
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
