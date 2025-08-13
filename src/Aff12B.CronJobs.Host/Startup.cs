using Aff12B.Common.Infrastructure.Middleware;
using Aff12B.CronJobs.Host.Extensions;
using Aff12B.CronJobs.Host.Filters;
using Aff12B.CronJobs.Host.Settings;
using Fanex.Data.SqlServer;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.HttpOverrides;
using Sentry.AspNetCore;
using StackExchange.Redis;

namespace Aff12B.CronJobs.Host
{
    public class Startup(IConfiguration configuration)
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFanexSql(configuration);
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.Configure<SentryAspNetCoreOptions>(configuration.GetSection("Sentry"));
            services.AddLogging();
            services.AddHttpClient();
            services.AddSiteHealthChecks(configuration);

            var appSetting = configuration.GetSection("App").Get<AppSetting>();

            services.AddApi(appSetting, configuration);
            AddHangFireService(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UsePing();
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
            app.UseMiddleware<ErrorHandler>();
            app.UseSiteHealthChecks(configuration);
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHangFire("/jobs");
            app.UseRecuringJobs(configuration);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.Map("/", async context =>
                {
                    await context.Response.WriteAsync("Hello, Welcome to 12B V2 CronJobs!");
                });
            });

        }

        private void AddHangFireService(IServiceCollection services)
        {
            var redisConfig = configuration.GetSection("Redis").Get<RedisSetting>();
            var redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                KeepAlive = redisConfig.KeepAlive,
                EndPoints = { { redisConfig.Server, redisConfig.Port } },
                Password = redisConfig.Pass,
                AbortOnConnectFail = false,
                DefaultDatabase = redisConfig.Database,
            });

            services.AddHangfire((serviceProvider, configuration) =>
            {
                configuration.UseRedisStorage(redis, new RedisStorageOptions
                {
                    Prefix = redisConfig.AppName,
                    Db = redisConfig.Database
                }).WithJobExpirationTimeout(TimeSpan.FromHours(2));
                configuration.UseFilter(serviceProvider.GetService<GlobalHangfireJobExceptionFilterAttribute>());
                configuration.UseFilter(serviceProvider.GetService<UMHangfireFilterAttribute>());
            });

            services.AddHangfireServer(options =>
            {
                options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
            });

            services.AddCronJobsDI(configuration);
        }
    }
}
