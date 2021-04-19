using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlazorApp.Data;
using PokemonCardCatalogue.Common.Context;
using PokemonCardCatalogue.Common.Context.Interfaces;
using System.Net.Http;
using PokemonCardCatalogue.Common.Logic;
using PokemonCardCatalogue.Common.Logic.Interfaces;

namespace BlazorApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();

            services.AddSingleton<HttpClient>((s) => new HttpClient
            {
                BaseAddress = new Uri($"https://api.pokemontcg.io/v2/")
            });
            services.AddSingleton<ICache>(ConfigureCache());
            services.AddSingleton<IApiService, PokemonTcgApiService>();
            services.AddSingleton<IApi, PokemonTcgApi>();
            services.AddSingleton<ICardCollection, DoNothingCardCollection>();
            services.AddSingleton<ICardLogic, CardLogic>();
            services.AddSingleton<IAllSetsLogic, AllSetsLogic>();
            services.AddSingleton<ISetListLogic, SetListLogic>();
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
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            PokemonCardCatalogue.Common.Self.SetApikey("8f8d3be5-5801-482e-97c0-4b7953b461fa");

        }

        private ICache ConfigureCache()
        {
            var sqliteCache = new SqliteCache();
            sqliteCache.Init();
            return sqliteCache;
        }
    }
}
