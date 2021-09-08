using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PensionManagementPortal.Filters;
using PensionManagementPortal.Models;
using PensionManagementPortal.Provider;
using PensionManagementPortal.Repository;
using PensionManagementPortal.Services;

namespace PensionManagementPortal
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
            services.AddControllersWithViews( options =>
            {
                options.Filters.Add<PensionExceptionFilter>();
            });

            services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(15);
                option.Cookie.HttpOnly = true;
            });

            // Session accessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Repository
            services.AddScoped<IProcessPensionRepository, ProcessPensionRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IPensionDbRepository, EFPensionDbRepository>();

            // Provider
            services.AddScoped<IEFProvider, SqlServerEFProvider>();

            // Services
            services.AddHttpClient<AuthService>(
                instance => {
                    instance.BaseAddress = new Uri(Configuration["ServiceUrls:Auth"]);
                    instance.DefaultRequestHeaders.Add("ContentType", "application/json");
                    instance.DefaultRequestHeaders.Add("Accept", "application/json");
                }
            );
            services.AddHttpClient<ProcessPensionService>(
                instance => {
                    instance.BaseAddress = new Uri(Configuration["ServiceUrls:ProcessPension"]);
                    instance.DefaultRequestHeaders.Add("ContentType", "application/json");
                    instance.DefaultRequestHeaders.Add("Accept", "application/json");
                }
            );

            //DbContext
            services.AddDbContext<PensionContext>(options =>
            {
                //options.UseSqlServer(Configuration.GetConnectionString("PensionContext"));
                options.UseInMemoryDatabase("InMemoryPension");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
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

            loggerFactory.AddLog4Net();

            app.UseSession();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Pension}/{action=Index}/{id?}");
            });
        }
    }
}
