using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ex.models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ex
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
            services.AddRazorPages();
            services.AddDbContext<MyBlogContext>(options =>
            {
                string connection = Configuration.GetConnectionString("MyBlogContext");
                options.UseSqlServer(connection);
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}

/*
    CREATE, READ, UPDATE, DELETE (CRUD)
    dotnet aspnet-codegenerator razorpage -m razorweb.models.Article -dc razorweb.models.MyBlogContext -outDir Pages/Blog -udl --referenceScriptLibraries

    dotnet aspnet-codegenerator razorpage -m ex.models.Article -dc ex.models.MyBlogContext -outDir Pages/Blog -udl --referenceScriptLibraries
*/