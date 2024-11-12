using Aff12B.CronJobs.Host.Extensions;
using Aff12B.CronJobs.Host.Filters;
using Aff12B.CronJobs.Host.Middleware;
using Aff12B.CronJobs.Host.Settings;
using Fanex.Data.SqlServer;
using Hangfire;
using Hangfire.Redis.StackExchange;
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
            services.AddLogging();
            services.AddUnderMaintenance(configuration);
            services.AddHttpClient();
            services.AddSiteHealthChecks(configuration);

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
            app.UseForwardedHeaders();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseHangFire("/jobs");
            app.UseRecuringJobs(configuration);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

        }

        private void AddHangFireService(IServiceCollection services)
        {
            var redisConfig = configuration.GetSection("Redis").Get<RedisSetting>();
            var redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                KeepAlive = 180,
                EndPoints = { { redisConfig.Server, redisConfig.Port } },
                Password = redisConfig.Pass,
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
