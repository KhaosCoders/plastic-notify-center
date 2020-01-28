using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using PlasticNotifyCenter.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlasticNotifyCenter.Filters;
using PlasticNotifyCenter.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using PlasticNotifyCenter.Authorization;
using PlasticNotifyCenter.Data.Identity;
using PlasticNotifyCenter.Notifiers;
using PlasticNotifyCenter.Services;
using Serilog;
using PlasticNotifyCenter.Services.Background;
using PlasticNotifyCenter.Data.Managers;

namespace PlasticNotifyCenter
{
    /// <summary>
    /// Sets up the dotnet core app
    /// </summary>
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
            Log.Debug("ConfigureServices");

            // Authorize-Handlers
            services.AddSingleton<IAuthorizationHandler,
                          AdministratorsAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler,
                          CoordinatorsAuthorizationHandler>();

            // Database (EF)
            services.AddDbContext<PncDbContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
#if DEBUG
                options.EnableSensitiveDataLogging(true);
#endif
            });

            // Data-Managers
            services.AddTransient<IAppSettingsManager, AppSettingsManager>();
            services.AddTransient<INotificationRulesManager, NotificationRulesManager>();
            services.AddTransient<ITriggerHistoryManager, TriggerHistoryManager>();
            services.AddTransient<INotificationHistoryManager, NotificationHistoryManager>();

            // App-Settings
            services.AddTransient<IAppSettingsService, AppSettingsService>();

            // LDAP
            services.AddTransient<ILdapService, LdapService>();
            services.AddHostedService<LdapSyncService>();

            // Identity
            services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<Role>()
                .AddEntityFrameworkStores<PncDbContext>();

            // Controllers
            services.AddControllersWithViews();

            // RazorPages
            services.AddRazorPages();

            // Default policy: User must be authenticated
            services.AddControllers(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                // SetupRedirectFilter before AuthorizeFilter
                config.Filters.Add(typeof(SetupRedirectFilter));
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            // Mail
            services.AddSingleton<IMailService, MailService>();

            // NotifierService
            services.AddSingleton<INotifierDefinitionService, NotifierDefinitionService>();
            services.AddSingleton<IRuleConditionEvaluator, RuleConditionEvaluator>();
            services.AddSingleton<INotificationQueue, NotificationQueue>();
            services.AddHostedService<NotifierService>();

            // Notifiers
            services.AddNotifiers();

            // MVC
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Log.Debug("Configure App");

            if (env.IsDevelopment())
            {
                // Show exception and database error pages in dev environment
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Serve static files uner wwwroot
            app.UseStaticFiles();

            // Use routing
            app.UseRouting();

            // authenticate and authoriye users
            app.UseAuthentication();
            app.UseAuthorization();

            // default endpoint
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

#if DEBUG
            try
            {
                // Write a trigger file while in debug mode, for browsersync to update the page on C# changes
                // Run programm with `dotnet watch run` for full browsersync experience
                System.IO.File.WriteAllText("browsersync-update.txt", DateTime.Now.ToString());
            }
            catch
            {
                // ignore
            }
#endif
        }
    }
}
