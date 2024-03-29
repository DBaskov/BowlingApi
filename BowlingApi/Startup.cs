﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BowlingApi.BusinessLogicHelpers;
using BowlingApi.DBContexts;
using BowlingApi.DBContexts.Models;
using BowlingApi.Repositories;
using BowlingApi.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BowlingApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<PlayersMongoDbSetings>(Configuration.GetSection(nameof(PlayersMongoDbSetings)));
            services.AddSingleton<IPlayersMongoDbSetings>(s => s.GetRequiredService<IOptions<PlayersMongoDbSetings>>().Value);
            services.TryAddSingleton<IMongoDBContext, MongoDBContext>();
            services.TryAddSingleton<IPlayerGameSessionsRepository, PlayerGameSessionMongoRepository>();
            services.TryAddSingleton<IPlayerGameSessionsHelper, PlayerGameSessionsHelper>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
