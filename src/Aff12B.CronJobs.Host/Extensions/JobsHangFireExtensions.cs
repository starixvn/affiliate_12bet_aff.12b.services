using Aff12B.Common.DataDomain.Params;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Host.Filters;
using Aff12B.CronJobs.Host.Settings;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Handlers;
using Aff12B.CronJobs.Infrastructure.Jobs;
using Aff12B.CronJobs.Infrastructure.Jobs.Bonus;
using Aff12B.CronJobs.Infrastructure.Jobs.Log;
using Aff12B.CronJobs.Infrastructure.Jobs.Product9W;
using Aff12B.CronJobs.Infrastructure.Jobs.RetryJob;
using Aff12B.CronJobs.Infrastructure.Repository;
using Aff12B.CronJobs.Infrastructure.Services;
using Fanex.Data.Repository;
using Hangfire;
using Refit;
using System.Linq.Expressions;
using System.Net;
using static Aff12B.CronJobs.Infrastructure.Constants.ServiceConfigName;

namespace Aff12B.CronJobs.Host.Extensions
{
    public static class JobsHangFireExtensions
    {
        private const string AppSettingRoot = "App";
        private const string JobSettingRoot = "JobSettings";

        public static IServiceCollection AddCronJobsDI(this IServiceCollection services, IConfiguration configuration)
        {
            var appSetting = configuration.GetSection("App").Get<AppSetting>();
            services.Configure<LicenseeSiteSetting>(configuration.GetSection($"{AppSettingRoot}:LicenseeSetting"));
            services.Configure<TimeTrackerSetting>(configuration.GetSection($"{AppSettingRoot}:TrackerSetting"));

            services.AddRepository()
                .AddService()
                .AddFilter()
                .AddApi(appSetting, configuration)
                .AddRecurringTasks(configuration);

            return services;
        }

        public static void UseHangFire(this IApplicationBuilder app, string path)
        {
            app.UseHangfireDashboard(path, new DashboardOptions
            {
                DisplayStorageConnectionString = false,
                Authorization = new[] { new AllowedAnonymousAuthorizationFilter() },
                IgnoreAntiforgeryToken = true
            });
        }

        public static void UseRecuringJobs(this IApplicationBuilder app, IConfiguration configuration)
        {
            var isTest = configuration.GetSection("App:IsTest").Get<bool>();
            if (!isTest)
            {
                RegisterFetchDataTask(configuration);
                RegisterRetryDataTask(configuration);
            }
        }

        private static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddScoped<IAffMemberRepository, AffMemberImportRepository>();
            services.AddScoped<IDataTrackerRepository, DataTrackerRepository>();
            services.AddScoped<IThirdPartyImportRepository, ThirdPartyImportRepository>();
            services.AddScoped<ITrackerRepository, TrackerRespository>();
            return services;
        }

        private static IServiceCollection AddService(this IServiceCollection services)
        {
            services.AddScoped<IDataTrackerService, DataTrackerService>();
            services.AddScoped<ITrackerService, TimeTrackerService>();
            return services;
        }

        private static IServiceCollection AddFilter(this IServiceCollection services)
        {
            services.AddSingleton<GlobalHangfireJobExceptionFilterAttribute>();
            services.AddSingleton<UMHangfireFilterAttribute>();
            return services;
        }

        private static IServiceCollection AddApi(this IServiceCollection services, AppSetting appSettings, IConfiguration configuration)
        {
            var proxySetting = configuration.GetSection("ProxySetting").Get<ProxySetting>();
            Func<HttpClientHandler> defaultFuncHttpHandler = () => new HttpClientHandler()
            {
                Proxy = proxySetting != null && proxySetting.IsUseProxy
                              ? new WebProxy(new Uri(proxySetting.ProxyUrl))
                              : null,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            services.Configure<TokenRequest>(configuration.GetSection("App:TokenRequest"));
            var setting = new RefitSettings();

            services.AddTransient<AuthHeaderHandler>();
            services.AddMemoryCache();

            services.AddRefitClient<ITokenApi>(setting)
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(appSettings.LicenseeDataAPI))
                .ConfigurePrimaryHttpMessageHandler(defaultFuncHttpHandler);

            services.AddRefitClient<ILicenseeApi>(setting)
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(appSettings.LicenseeDataAPI))
                .AddHttpMessageHandler<AuthHeaderHandler>()
                .ConfigurePrimaryHttpMessageHandler(defaultFuncHttpHandler);

            services.AddRefitClient<IProduct9WApi>(setting)
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(appSettings.LicenseeDataAPI))
                .AddHttpMessageHandler<AuthHeaderHandler>()
                .ConfigurePrimaryHttpMessageHandler(defaultFuncHttpHandler);
            return services;
        }

        private static void AddRecurringTasks(this IServiceCollection services, IConfiguration configuration)
        {
            var listJobDic = configuration.GetSection(JobSettingRoot).GetChildren().ToDictionary(_ => _.Key, _ => _);

            //FetchCustomerDataTask
            services.AddScoped<FetchAffCustomerDataTask>();

            //FetchBetRecordDataTask
            services.AddScoped<FetchBetRecordDataTask>();
            services.Configure<FetchServiceSetting>(
                ServiceConfigName.PULL_BET_RECORD_SETTING,
                listJobDic[ServiceConfigName.PULL_BET_RECORD_SETTING]);

            //FetchCustomerDataTask
            services.AddScoped<FetchCustomerDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_CUSTOMER_SETTING,
                    listJobDic[ServiceConfigName.PULL_CUSTOMER_SETTING]);

            //FetchServerInfoDataTask
            services.AddScoped<FetchServerInfoDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_SERVER_INFO_SETTING,
                    listJobDic[ServiceConfigName.PULL_SERVER_INFO_SETTING]);

            //FetchTpGameTypeDataTask
            services.AddScoped<FetchGameTypeDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_GAME_TYPE_SETTING,
                    listJobDic[ServiceConfigName.PULL_GAME_TYPE_SETTING]);

            //FetchDepositInfoDataTask
            services.AddScoped<FetchDepositInfoDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_DEPOSIT_INFO_SETTING,
                    listJobDic[ServiceConfigName.PULL_DEPOSIT_INFO_SETTING]);

            //FetchBonusInfoDataTask
            services.AddScoped<FetchBonusInfoDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_BONUS_INFO_SETTING,
                    listJobDic[ServiceConfigName.PULL_BONUS_INFO_SETTING]);

            services.AddScoped<FetchWeeklyBonusInfoDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_WEEKLY_BONUS_INFO_SETTING,
                    listJobDic[ServiceConfigName.PULL_WEEKLY_BONUS_INFO_SETTING]);


            //FetchWithdrawalInfoDataTask
            services.AddScoped<FetchWithdrawalInfoDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_WITHDRAWAL_INFO,
                    listJobDic[ServiceConfigName.PULL_WITHDRAWAL_INFO]
                );

            //FetchRebateMLogDataTask
            services.AddScoped<FetchRebateMLogDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_REBATEMLOG_SETTING,
                    listJobDic[ServiceConfigName.PULL_REBATEMLOG_SETTING]
                );

            services.AddScoped<FetchTpReport9WDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_TPREPORT_9W_SETTING,
                    listJobDic[ServiceConfigName.PULL_TPREPORT_9W_SETTING]
                );

            services.AddScoped<FetchTpWager9WDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_TPWAGER_9W_SETTING,
                    listJobDic[ServiceConfigName.PULL_TPWAGER_9W_SETTING]
                );

            services.AddScoped<ReInsertAffiliateInfoDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.INSERT_AFFILIATE_SETTING,
                    listJobDic[ServiceConfigName.INSERT_AFFILIATE_SETTING]
                );
        }

        private static void RegisterFetchDataTask(IConfiguration configuration)
        {
            var listJobDic = configuration.GetSection(JobSettingRoot).GetChildren().ToDictionary(_ => _.Key, _ => _.Get<FetchServiceSetting>());

            RegisterTask<FetchAffCustomerDataTask>(typeof(FetchAffCustomerDataTask).Name, listJobDic[PULL_AFF_CUSTOMER_SETTING].PullInfoCron, job => job.FetchDataAsync());

            RegisterTask<FetchBetRecordDataTask>($"{typeof(FetchBetRecordDataTask).Name}", listJobDic[PULL_BET_RECORD_SETTING].PullInfoCron, job => job.FetchDataAsync());
            RegisterTask<FetchBetRecordDataTask>($"{typeof(FetchBetRecordDataTask).Name}-2HourInterval", "30 0/2 ? * *", job => job.FetchAllDay(2));
            RegisterTask<FetchBetRecordDataTask>($"{typeof(FetchBetRecordDataTask).Name}-Daily", Cron.Daily(7), job => job.FetchFromAdjusmentDayAsync(3));
            RegisterTask<FetchBetRecordDataTask>($"{typeof(FetchBetRecordDataTask).Name}-Monthly", Cron.Monthly(4, 12, 0), job => job.FetchLastMonthAsync());

            RegisterTask<FetchCustomerDataTask>($"{typeof(FetchCustomerDataTask).Name}", listJobDic[PULL_CUSTOMER_SETTING].PullInfoCron, job => job.FetchDataAsync());
            RegisterTask<FetchCustomerDataTask>($"{typeof(FetchCustomerDataTask).Name}-Daily", Cron.Daily(5), job => job.FetchDataDailyTask());

            RegisterTask<FetchServerInfoDataTask>($"{typeof(FetchServerInfoDataTask).Name}", listJobDic[PULL_SERVER_INFO_SETTING].PullInfoCron, job => job.FetchDataAsync());

            RegisterTask<FetchGameTypeDataTask>($"{typeof(FetchGameTypeDataTask).Name}", listJobDic[PULL_GAME_TYPE_SETTING].PullInfoCron, job => job.FetchDataAsync());

            RegisterTask<FetchDepositInfoDataTask>($"{typeof(FetchDepositInfoDataTask).Name}", listJobDic[PULL_DEPOSIT_INFO_SETTING].PullInfoCron, job => job.FetchDataAsync());

            //Bonus Info
            RegisterTask<FetchBonusInfoDataTask>($"{typeof(FetchBonusInfoDataTask).Name}", listJobDic[PULL_BONUS_INFO_SETTING].PullInfoCron, job => job.FetchDataAsync());
            RegisterTask<FetchWeeklyBonusInfoDataTask>($"{typeof(FetchWeeklyBonusInfoDataTask).Name}", listJobDic[PULL_WEEKLY_BONUS_INFO_SETTING].PullInfoCron, job => job.FetchDataAsync());


            RegisterTask<FetchWithdrawalInfoDataTask>($"{typeof(FetchWithdrawalInfoDataTask).Name}", listJobDic[PULL_WITHDRAWAL_INFO].PullInfoCron, job => job.FetchDataAsync());

            RegisterTask<FetchRebateMLogDataTask>($"{typeof(FetchRebateMLogDataTask).Name}", listJobDic[PULL_REBATEMLOG_SETTING].PullInfoCron, job => job.FetchDataAsync());

            RegisterTask<FetchTpReport9WDataTask>($"{typeof(FetchTpReport9WDataTask).Name}", listJobDic[PULL_TPREPORT_9W_SETTING].PullInfoCron, job => job.FetchDataAsync());

            RegisterTask<FetchTpWager9WDataTask>($"{typeof(FetchTpWager9WDataTask).Name}", listJobDic[PULL_TPWAGER_9W_SETTING].PullInfoCron, job => job.FetchDataAsync());
        }

        private static void RegisterRetryDataTask(IConfiguration configuration)
        {
            var supportedPrefix = configuration.GetSection("App").Get<AppSetting>().SupportDBPrefix;
            var listJobDic = configuration.GetSection(JobSettingRoot).GetChildren().ToDictionary(_ => _.Key, _ => _.Get<FetchServiceSetting>());
            RegisterTask<ReInsertAffiliateInfoDataTask>($"{typeof(ReInsertAffiliateInfoDataTask).Name}", listJobDic[INSERT_AFFILIATE_SETTING].PullInfoCron, job => job.HandleAsync());
        }

        private static void RegisterTask<T>(string serviceId, string cronExpression, Expression<Func<T, Task>> methodCall)
        {
            if (!string.IsNullOrWhiteSpace(cronExpression))
            {
                RecurringJob.AddOrUpdate<T>(
                            serviceId,
                            methodCall,
                            cronExpression);
            }
        }

    }
}
