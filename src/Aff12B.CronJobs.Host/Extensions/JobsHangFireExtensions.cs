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
using Aff12B.CronJobs.Infrastructure.Jobs.AffCustomer;
using Aff12B.CronJobs.Infrastructure.Jobs.Bonus;
using Aff12B.CronJobs.Infrastructure.Jobs.Customer;
using Aff12B.CronJobs.Infrastructure.Jobs.Deposit;
using Aff12B.CronJobs.Infrastructure.Jobs.Log;
using Aff12B.CronJobs.Infrastructure.Jobs.Product9W;
using Aff12B.CronJobs.Infrastructure.Jobs.RetryJob;
using Aff12B.CronJobs.Infrastructure.Repository;
using Aff12B.CronJobs.Infrastructure.Services;
using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Hangfire;
using ONEbook.UM.DependencyInjection;
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
                .AddService(configuration)
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

        public static void UseRecuringJobs(this IApplicationBuilder _, IConfiguration configuration)
        {
            var isTest = configuration.GetSection("App:IsTest").Get<bool>();
            if (!isTest)
            {
                var listJobDic = configuration.GetSection(JobSettingRoot).GetChildren().ToDictionary(_ => _.Key, _ => _.Get<FetchServiceSetting>());

                RegisterFetchDataTask(listJobDic);
                RegisterRetryDataTask(configuration, listJobDic);
                RegisterUpdateMasterDataTask(listJobDic);
            }
        }

        private static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddScoped<IAffMemberRepository, AffMemberImportRepository>();
            services.AddScoped<IDataTrackerRepository, DataTrackerRepository>();
            services.AddScoped<IThirdPartyImportRepository, ThirdPartyImportRepository>();
            services.AddScoped<ITrackerRepository, TrackerRepository>();
            services.AddScoped<IAffInfoRepository, AffInfoRepository>();
            return services;
        }

        private static IServiceCollection AddService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDataTrackerService, DataTrackerService>();
            services.AddScoped<ITrackerService, TimeTrackerService>();

            // Add Fanex Service
            services.AddSingleton<ICacheService, CacheService>();
            services.AddRedisCache(configuration.GetSection("fanex.caching.redis"));
            services.AddOneBookUmForService(configuration);

            return services;
        }

        private static IServiceCollection AddFilter(this IServiceCollection services)
        {
            services.AddSingleton<GlobalHangfireJobExceptionFilterAttribute>();
            services.AddSingleton<UMHangfireFilterAttribute>();
            return services;
        }

        public static IServiceCollection AddApi(this IServiceCollection services, AppSetting appSettings, IConfiguration configuration)
        {
            var proxySetting = configuration.GetSection("ProxySetting").Get<ProxySetting>();
            Func<HttpClientHandler> defaultFuncHttpHandler = () => new HttpClientHandler
            {
                Proxy = proxySetting != null && proxySetting.IsUseProxy
                              ? new WebProxy(new Uri(proxySetting.ProxyUrl))
                              : null,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            services.Configure<TokenRequest>(configuration.GetSection("App:TokenRequest"));
            var setting = new RefitSettings();

            services.AddTransient<AuthHeaderHandler>();
            Microsoft.Extensions.DependencyInjection.MemoryCacheServiceCollectionExtensions.AddMemoryCache(services);

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

            AddCustomerTasks(services, listJobDic);
            AddBetRecordTasks(services, listJobDic);
            AddServerInfoTasks(services, listJobDic);
            AddGameTypeTasks(services, listJobDic);
            AddDepositInfoTasks(services, listJobDic);
            AddBonusInfoTasks(services, listJobDic);
            AddWithdrawalInfoTasks(services, listJobDic);
            AddRebateMLogTasks(services, listJobDic);
            AddTpReport9WTasks(services, listJobDic);
            AddTpWager9WTasks(services, listJobDic);
            AddAffiliateTasks(services, listJobDic);
            AddMasterDataCacheTasks(services, listJobDic);
            AddTagTasks(services, listJobDic);
        }

        private static void AddCustomerTasks(IServiceCollection services, Dictionary<string, IConfigurationSection> listJobDic)
        {
            // FetchAffCustomerDataTask
            services.AddScoped<FetchAffCustomerDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_AFF_CUSTOMER_SETTING,
                    listJobDic[ServiceConfigName.PULL_AFF_CUSTOMER_SETTING]);

            // FetchRealAffCustomerDataTask
            services.AddScoped<FetchRealAffCustomerDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_REAL_AFF_CUSTOMER_SETTING,
                    listJobDic[ServiceConfigName.PULL_REAL_AFF_CUSTOMER_SETTING]);

            // FetchCustomerDataTask
            services.AddScoped<FetchCustomerDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_CUSTOMER_SETTING,
                    listJobDic[ServiceConfigName.PULL_CUSTOMER_SETTING]);

            // FetchRealCustomerDataTask
            services.AddScoped<FetchRealCustomerDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_REAL_CUSTOMER_SETTING,
                    listJobDic[ServiceConfigName.PULL_REAL_CUSTOMER_SETTING]);
        }

        private static void AddBetRecordTasks(IServiceCollection services, Dictionary<string, IConfigurationSection> listJobDic)
        {
            // FetchBetRecordDataTask
            services.AddScoped<FetchBetRecordDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_BET_RECORD_SETTING,
                    listJobDic[ServiceConfigName.PULL_BET_RECORD_SETTING]);
        }

        private static void AddServerInfoTasks(IServiceCollection services, Dictionary<string, IConfigurationSection> listJobDic)
        {
            // FetchServerInfoDataTask
            services.AddScoped<FetchServerInfoDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_SERVER_INFO_SETTING,
                    listJobDic[ServiceConfigName.PULL_SERVER_INFO_SETTING]);
        }

        private static void AddGameTypeTasks(IServiceCollection services, Dictionary<string, IConfigurationSection> listJobDic)
        {
            // FetchGameTypeDataTask
            services.AddScoped<FetchGameTypeDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_GAME_TYPE_SETTING,
                    listJobDic[ServiceConfigName.PULL_GAME_TYPE_SETTING]);
        }

        private static void AddDepositInfoTasks(IServiceCollection services, Dictionary<string, IConfigurationSection> listJobDic)
        {
            // FetchDepositInfoDataTask
            services.AddScoped<FetchDepositInfoDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_DEPOSIT_INFO_SETTING,
                    listJobDic[ServiceConfigName.PULL_DEPOSIT_INFO_SETTING]);

            // FetchRealDepositInfoDataTask
            services.AddScoped<FetchRealDepositInfoDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_REAL_DEPOSIT_INFO_SETTING,
                    listJobDic[ServiceConfigName.PULL_REAL_DEPOSIT_INFO_SETTING]);
        }

        private static void AddBonusInfoTasks(IServiceCollection services, Dictionary<string, IConfigurationSection> listJobDic)
        {
            // FetchBonusInfoDataTask
            services.AddScoped<FetchBonusInfoDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_BONUS_INFO_SETTING,
                    listJobDic[ServiceConfigName.PULL_BONUS_INFO_SETTING]);

            // FetchWeeklyBonusInfoDataTask
            services.AddScoped<FetchWeeklyBonusInfoDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_WEEKLY_BONUS_INFO_SETTING,
                    listJobDic[ServiceConfigName.PULL_WEEKLY_BONUS_INFO_SETTING]);
        }

        private static void AddWithdrawalInfoTasks(IServiceCollection services, Dictionary<string, IConfigurationSection> listJobDic)
        {
            // FetchWithdrawalInfoDataTask
            services.AddScoped<FetchWithdrawalInfoDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_WITHDRAWAL_INFO,
                    listJobDic[ServiceConfigName.PULL_WITHDRAWAL_INFO]);
        }

        private static void AddRebateMLogTasks(IServiceCollection services, Dictionary<string, IConfigurationSection> listJobDic)
        {
            // FetchRebateMLogDataTask
            services.AddScoped<FetchRebateMLogDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_REBATEMLOG_SETTING,
                    listJobDic[ServiceConfigName.PULL_REBATEMLOG_SETTING]);
        }

        private static void AddTpReport9WTasks(IServiceCollection services, Dictionary<string, IConfigurationSection> listJobDic)
        {
            // FetchTpReport9WDataTask
            services.AddScoped<FetchTpReport9WDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_TPREPORT_9W_SETTING,
                    listJobDic[ServiceConfigName.PULL_TPREPORT_9W_SETTING]);
        }

        private static void AddTpWager9WTasks(IServiceCollection services, Dictionary<string, IConfigurationSection> listJobDic)
        {
            // FetchTpWager9WDataTask
            services.AddScoped<FetchTpWager9WDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_TPWAGER_9W_SETTING,
                    listJobDic[ServiceConfigName.PULL_TPWAGER_9W_SETTING]);
        }

        private static void AddAffiliateTasks(IServiceCollection services, Dictionary<string, IConfigurationSection> listJobDic)
        {
            // ReInsertAffiliateInfoDataTask
            services.AddScoped<ReInsertAffiliateInfoDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.INSERT_AFFILIATE_SETTING,
                    listJobDic[ServiceConfigName.INSERT_AFFILIATE_SETTING]);

            // InitialInsertAffInfoDataTask
            services.AddScoped<InitialInsertAffInfoDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.INITIAL_INSERT_AFFILIATE_SETTING,
                    listJobDic[ServiceConfigName.INITIAL_INSERT_AFFILIATE_SETTING]);
        }

        private static void AddMasterDataCacheTasks(IServiceCollection services, Dictionary<string, IConfigurationSection> listJobDic)
        {
            // UpdateMasterDataCacheTask
            services.AddScoped<UpdateMasterDataCacheTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.UPDATE_MASTER_DATA_CACHE_SETTING,
                    listJobDic[ServiceConfigName.UPDATE_MASTER_DATA_CACHE_SETTING]);
        }

        private static void AddTagTasks(IServiceCollection services, Dictionary<string, IConfigurationSection> listJobDic)
        {
            // FetchTagDataTask
            services.AddScoped<FetchTagDataTask>()
                .Configure<FetchServiceSetting>(
                    ServiceConfigName.PULL_TAG_SETTING,
                    listJobDic[ServiceConfigName.PULL_TAG_SETTING]);
        }

        private static void RegisterFetchDataTask(Dictionary<string, FetchServiceSetting> listJobDic)
        {
            RegisterTask<FetchRealAffCustomerDataTask>("Fetch Aff Customer Data -- Real", listJobDic[PULL_REAL_AFF_CUSTOMER_SETTING].PullInfoCron, job => job.FetchDataAsync());
            RegisterTask<FetchAffCustomerDataTask>("Fetch Aff Customer Data -- Normal", listJobDic[PULL_AFF_CUSTOMER_SETTING].PullInfoCron, job => job.FetchDataAsync());
            RegisterTask<FetchAffCustomerDataTask>("Fetch Aff Customer Data -- DATE_RANGE", Cron.Never(), job => job.FetchDataByDateRangeTask());

            RegisterTask<FetchBetRecordDataTask>($"{typeof(FetchBetRecordDataTask).Name}", listJobDic[PULL_BET_RECORD_SETTING].PullInfoCron, job => job.FetchDataAsync());
            RegisterTask<FetchBetRecordDataTask>($"{typeof(FetchBetRecordDataTask).Name}-2HourInterval", "20 0/2 ? * *", job => job.FetchAllDay(2));
            RegisterTask<FetchBetRecordDataTask>($"{typeof(FetchBetRecordDataTask).Name}-Daily", Cron.Daily(7, 5), job => job.FetchFromAdjusmentDayAsync(3));
            RegisterTask<FetchBetRecordDataTask>($"{typeof(FetchBetRecordDataTask).Name}-Monthly", Cron.Monthly(4, 12, 0), job => job.FetchLastMonthAsync());

            RegisterTask<FetchRealCustomerDataTask>("Fetch Customer Data -- Real", listJobDic[PULL_REAL_CUSTOMER_SETTING].PullInfoCron, job => job.FetchDataAsync());
            RegisterTask<FetchCustomerDataTask>("Fetch Customer Data -- Normal", listJobDic[PULL_CUSTOMER_SETTING].PullInfoCron, job => job.FetchDataAsync());
            RegisterTask<FetchCustomerDataTask>("Fetch Customer Data -- Daily", Cron.Daily(5), job => job.FetchDataDailyTask());
            RegisterTask<FetchCustomerDataTask>("Fetch Customer Data -- DATE_RANGE", Cron.Never(), job => job.FetchDataByDateRangeTask());

            RegisterTask<FetchServerInfoDataTask>($"{typeof(FetchServerInfoDataTask).Name}", listJobDic[PULL_SERVER_INFO_SETTING].PullInfoCron, job => job.FetchDataAsync());

            RegisterTask<FetchGameTypeDataTask>($"{typeof(FetchGameTypeDataTask).Name}", listJobDic[PULL_GAME_TYPE_SETTING].PullInfoCron, job => job.FetchDataAsync());

            RegisterTask<FetchRealDepositInfoDataTask>("Fetch Deposit Info Data -- Real", listJobDic[PULL_REAL_DEPOSIT_INFO_SETTING].PullInfoCron, job => job.FetchDataAsync());
            RegisterTask<FetchDepositInfoDataTask>("Fetch Deposit Info Data -- Normal", listJobDic[PULL_DEPOSIT_INFO_SETTING].PullInfoCron, job => job.FetchDataAsync());

            //Bonus Info
            RegisterTask<FetchBonusInfoDataTask>($"{typeof(FetchBonusInfoDataTask).Name}", listJobDic[PULL_BONUS_INFO_SETTING].PullInfoCron, job => job.FetchDataAsync());
            RegisterTask<FetchWeeklyBonusInfoDataTask>($"{typeof(FetchWeeklyBonusInfoDataTask).Name}", listJobDic[PULL_WEEKLY_BONUS_INFO_SETTING].PullInfoCron, job => job.FetchDataAsync());


            RegisterTask<FetchWithdrawalInfoDataTask>($"{typeof(FetchWithdrawalInfoDataTask).Name}", listJobDic[PULL_WITHDRAWAL_INFO].PullInfoCron, job => job.FetchDataAsync());

            RegisterTask<FetchRebateMLogDataTask>($"{typeof(FetchRebateMLogDataTask).Name}", listJobDic[PULL_REBATEMLOG_SETTING].PullInfoCron, job => job.FetchDataAsync());

            RegisterTask<FetchTpReport9WDataTask>($"{typeof(FetchTpReport9WDataTask).Name}", listJobDic[PULL_TPREPORT_9W_SETTING].PullInfoCron, job => job.FetchDataAsync());

            RegisterTask<FetchTpWager9WDataTask>($"{typeof(FetchTpWager9WDataTask).Name}", listJobDic[PULL_TPWAGER_9W_SETTING].PullInfoCron, job => job.FetchDataAsync());
        }

        private static void RegisterRetryDataTask(IConfiguration configuration, Dictionary<string, FetchServiceSetting> listJobDic)
        {
            var appSetting = configuration.GetSection("App").Get<AppSetting>();

            if (appSetting.IsApiDataModifiable)
            {
                RegisterTask<ReInsertAffiliateInfoDataTask>($"{typeof(ReInsertAffiliateInfoDataTask).Name}", listJobDic[INSERT_AFFILIATE_SETTING].PullInfoCron, job => job.HandleAsync());
                RegisterTask<InitialInsertAffInfoDataTask>(typeof(InitialInsertAffInfoDataTask).Name, listJobDic[INITIAL_INSERT_AFFILIATE_SETTING].PullInfoCron, job => job.HandleAsync());
            }
        }

        private static void RegisterUpdateMasterDataTask(Dictionary<string, FetchServiceSetting> listJobDic)
        {
            // Register the cache update task - runs every 10 minutes
            RegisterTask<UpdateMasterDataCacheTask>($"{typeof(UpdateMasterDataCacheTask).Name}", listJobDic[UPDATE_MASTER_DATA_CACHE_SETTING].PullInfoCron, job => job.UpdateMasterDataCacheAsync());
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
