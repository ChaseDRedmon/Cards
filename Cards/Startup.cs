using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cards.Configuration;
using Cards.Data;
using Cards.Data.Models;
using Cards.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VueCliMiddleware;

namespace Cards
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
            services.AddControllers();
        
            // Add our config class to our configuration
            services.Configure<CardsConfig>(Configuration);

            // Create and persist our data protection keys to a directory
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(@"dataprotection"));

            // Add cookie authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/api/unauthorized";
                    //options.LogoutPath = "/logout";
                    options.ExpireTimeSpan = new TimeSpan(7, 0, 0, 0);
                });

            // Add anti-forgery tokens
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            // Add response compression
            services.AddResponseCompression();

            // Add a transient service to our static file configuration extension. This setups up anti-forgery
            // This runs automatically
            services.AddTransient<IConfigureOptions<StaticFileOptions>, StaticFilesConfiguration>();

            // Add a transient service to our Config Validator. The config validator ensures that our parameters are set. 
            // This runs automatically
            services.AddTransient<IStartupFilter, OpenDndConfigValidator>();

            // Create our database service context and tell the application to use SQL Server 
            services.AddDbContext<CardsContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetValue<string>(nameof(CardsConfig.DbConnection)));
                });

            // Add additional services
            services.AddOpenDND();

            // Add MVC services
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
            
            // In production, the VueJS files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "..\\Cards.Web\\clientapp\\dist";
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
            }
            
            // HTTP Strict Transport Security header
            //app.UseHsts();
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // Use cookie auth
            //app.UseAuthentication();

            // Use response compression
            app.UseResponseCompression();

            //app.UseStaticFiles(new StaticFileOptions { ServeUnknownFileTypes = true});
            app.UseSpaStaticFiles();
            //app.UseMvcWithDefaultRoute();

            // SPA routes
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "..\\Cards.Web\\clientapp";

                if (env.IsDevelopment())
                {
                    // run npm process with client app
                    spa.UseVueCli(npmScript: "serve", port: 8080, regex: "Compiled ");
                    // if you just prefer to proxy requests from client app, use proxy to SPA dev server instead:
                    // app should be already running before starting a .NET client
                    // spa.UseProxyToSpaDevelopmentServer("http://localhost:8080"); // your Vue app port
                }
            });
        }
    }
}