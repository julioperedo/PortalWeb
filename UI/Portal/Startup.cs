using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Portal.Interfaces;
using Portal.Membership;
using Portal.Misc;
using Portal.Models;
using Portal.Services;
using Telerik.Reporting.Cache.File;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.AspNetCore;

namespace Portal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private IServiceCollection services;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            this.services = services;

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.Name = ".PortalV3.Session";
            });

            services.AddTransient<IEmailSender, EmailSender>();            

            var cultureInfo = new CultureInfo("es-BO");
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
            cultureInfo.NumberFormat.NumberGroupSeparator = ",";
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(culture: cultureInfo, uiCulture: cultureInfo);
                options.SupportedCultures = new List<CultureInfo> { cultureInfo };
                options.SupportedUICultures = new List<CultureInfo> { cultureInfo };
            });

            services.AddMvc().AddMvcOptions(options => options.EnableEndpointRouting = false); //.SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddSingleton<ConfigurationService>();
            services.Configure<BEntities.Settings.SAPSettings>(Configuration);
            services.AddCustomMembership<CustomMembership>((options) =>
            {
                options.AuthenticationType = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultPathAfterLogin = "/";
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie((options) =>
                {
                    options.LoginPath = new PathString("/account/login");
                    options.LogoutPath = new PathString("/account/logout");
                    options.Events = new CookieAuthenticationEvents()
                    {
                        OnValidatePrincipal = async (c) =>
                        {
                            var membership = c.HttpContext.RequestServices.GetRequiredService<ICustomMembership>();
                            var isValid = await membership.ValidateLoginAsync(c.Principal);
                            if (!isValid)
                            {
                                c.RejectPrincipal();
                            }
                        }
                    };
                });
            services.AddKendo();

            services.AddRouting();
            services.AddControllers(); //.AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            services.AddControllersWithViews();

            services.Configure<IISServerOptions>(options => options.AllowSynchronousIO = true);

            services.AddRazorPages().AddNewtonsoftJson();

            // Configure dependencies for ReportsController.
            services.TryAddSingleton<IReportServiceConfiguration>(sp =>
                new ReportServiceConfiguration
                {
                    ReportingEngineConfiguration = ConfigurationHelper.ResolveConfiguration(sp.GetService<IWebHostEnvironment>()),
                    HostAppId = "PortalV311",
                    Storage = new FileStorage(Path.Combine(sp.GetService<IWebHostEnvironment>().WebRootPath, "reportsCache")),
                    ReportSourceResolver = new UriReportSourceResolver(Path.Combine(sp.GetService<IWebHostEnvironment>().WebRootPath, "reports")),
                    //ReportSourceResolver = new CustomReportResolver(Path.Combine(sp.GetService<IWebHostEnvironment>().WebRootPath, "reports"), Configuration.GetSection("ReportSettings").Get<ReportSettings>().ServiceName),
                    ClientSessionTimeout = 480,
                    ReportSharingTimeout = 480                    
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Home/NotFound";
                    await next();
                }
            });


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseRequestLocalization();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    public class ConfigurationService
    {
        public IConfiguration Configuration { get; private set; }

        public IWebHostEnvironment Environment { get; private set; }
        public ConfigurationService(IWebHostEnvironment environment)
        {
            this.Environment = environment;

            var configFileName = System.IO.Path.Combine(environment.ContentRootPath, "appsettings.json");
            var config = new ConfigurationBuilder().AddJsonFile(configFileName, true).Build();

            this.Configuration = config;
        }

        public ConfigurationService(IWebHostEnvironment environment, IConfiguration config)
        {
            this.Environment = environment;
            this.Configuration = config;
        }

    }

    static class ConfigurationHelper
    {
        public static IConfiguration ResolveConfiguration(IWebHostEnvironment environment)
        {
            var reportingConfigFileName = System.IO.Path.Combine(environment.ContentRootPath, "appsettings.json");
            return new ConfigurationBuilder().AddJsonFile(reportingConfigFileName, true).Build();
        }
    }

}
