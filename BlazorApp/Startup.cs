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
            _ = services.AddRazorPages();
            _ =services.AddServerSideBlazor();

            _ = services.AddSingleton<WeatherForecastService>()
                .AddSingleton((s) => new HttpClient
                {
                    BaseAddress = new Uri($"https://api.pokemontcg.io/v2/")
                })
                .AddSingleton<IDatabaseService, SqliteDatabaseService>()
                .AddSingleton<ICache, DbCache>()
                .AddSingleton<IApiService, PokemonTcgApiService>()
                .AddSingleton<IApi, PokemonTcgApi>()
                .AddSingleton<ICardCollection, DoNothingCardCollection>()
                .AddSingleton<ICardLogic, CardLogic>()
                .AddSingleton<IAllSetsLogic, AllSetsLogic>()
                .AddSingleton<ISetListLogic, SetListLogic>();

            Configuration.Get<ICache>()
                .Init();
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
    }
}
