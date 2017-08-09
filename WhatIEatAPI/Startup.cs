using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WhatIEatAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Caching.Memory;
using Swashbuckle.AspNetCore.Swagger;
using NLog.Extensions.Logging;

namespace WhatIEatAPI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddMemoryCache();
            services.AddEntityFrameworkSqlServer();
            services.AddDbContext<WhatIEatDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // To fix "Multipart body length limit 16384 exceeded" exception
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
                x.MemoryBufferThreshold = int.MaxValue;
            });
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IMemoryCache cache)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // .net core does not provide a build in logging to a file therefore we have to use external provide, NLog in this case
            //loggerFactory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());
            loggerFactory.AddNLog();

            app.UseDeveloperExceptionPage();

            // Populate application level cache (will be loaded from the database)
            // Make sure all values are unique
            // !!! place for the optimization - knowledge base should be ordered in a way that more frequent ingredients apear higher in the list

            //List<string> knowledgeBaseShort = new List<string>(new string[] {
            //        "sugar", "salt", "honey", "soya", "egg", "milk"
            //    });

            //List<string> knowledgeBaseLong = new List<string>(new string[] {
            //        "wheat flour", "oat flakes", "wholegrain wheat flour", "cereals", "wholegrain barley flour",
            //        "wholegrain spelt flour", "wholegrain rye flour", "rapeseed oil", "magnesium carbonate",
            //        "chocolate chips", "sodium hydrogen carbonate", "ammonium hydrogen", "wholegrain cereals",
            //        "sugar", "salt", "honey", "soya", "egg", "milk", "cocoa paste", "cocoa butter", "emulsifier",
            //        "soyalecithin", "hazelnuts", "raising agents", "flavourings", "elemental iron",
            //        "acidity regulator"
            //    });


            var entryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
            

            using (WhatIEatDbContext db = new WhatIEatDbContext())
            {
                // Populate cache of short names from the database
                var ingShort = db.Ingredients.Where(i => i.IngredientName.Length < 5).Select(i => i.IngredientName).ToList();
                cache.Set("knowledgeBaseShort", ingShort, entryOptions); // short words

                // Populate cache of all names from the database
                var ingAll = db.Ingredients.Select(i => i.IngredientName).ToList();
                cache.Set("knowledgeBaseAll", ingAll, entryOptions); // long words
                //cache.Set("knowledgeBaseLong", knowledgeBaseLong, entryOptions); // long words

                // Populate cache of all names from the database
                IEnumerable<Ingredient> ingEverything = db.Ingredients.Select(l => l).ToList<Ingredient>();
                cache.Set<IEnumerable<Ingredient>>("knowledgeBaseEverything", ingEverything, entryOptions); // long words
                
            }

            //app.UseMvc();
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(name: "default",
            //                    template: "{controller=Home}/{action=Index}/{id?}");
            //});
            app.UseMvcWithDefaultRoute();

            

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
