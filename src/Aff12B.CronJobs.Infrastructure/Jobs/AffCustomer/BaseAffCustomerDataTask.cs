using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Exceptions;
using Aff12B.CronJobs.Infrastructure.Helpers;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Web;

namespace Aff12B.CronJobs.Infrastructure.Jobs.AffCustomer
{
    public abstract class BaseAffCustomerDataTask
    {
        protected const int BATCH_COUNT = 2000;
        protected readonly IAffMemberRepository _importerRepository;
        protected readonly ILicenseeApi _licenseeApi;
        protected readonly ILogger _logger;
        protected readonly FetchServiceSetting _fetchServiceSetting;
        protected readonly LicenseeSiteSetting _licenseeSiteSetting;

        protected IEnumerable<string> SupportedSites => _licenseeSiteSetting?.SupportedSites;

        public BaseAffCustomerDataTask(ILicenseeApi licenseeApi
            , IAffMemberRepository memberImportRepository
            , ILogger<BaseAffCustomerDataTask> logger
            , LicenseeSiteSetting licenseeSiteSetting
            , FetchServiceSetting fetchServiceSetting)
        {
            _importerRepository = memberImportRepository;
            _licenseeApi = licenseeApi;
            _logger = logger;
            _licenseeSiteSetting = licenseeSiteSetting ?? throw new ArgumentNullException(nameof(licenseeSiteSetting));
            _fetchServiceSetting = fetchServiceSetting ?? throw new ArgumentNullException(nameof(fetchServiceSetting));
        }

        public async Task FetchDataAsync()
        {
            if (SupportedSites == null
                || !SupportedSites.Any())
            {
                return;
            }

            var maxTimeRange = GetApiMaxTimeRange();

            await FetchDataAsync(maxTimeRange);
        }

        protected async Task FetchDataAsync(TimeSpan maxTimeRange)
        {
            var date = await GetMinLastedTimeAsync();
            var fromDate = GetAdjustmentTime(date);

            var apiServerNow = DateTimeHelper.GetApiServerDateTime();
            var dateRanges = DateTimeHelper.GetProportionTimeRange(fromDate, apiServerNow, maxTimeRange);

            await HandleFetchDataAsync(dateRanges);
        }

        protected async Task<DateTime> GetMinLastedTimeAsync()
        {
            var dates = new List<DateTime>();
            foreach (var site in SupportedSites)
            {
                try
                {
                    var siteCode = _licenseeSiteSetting.GetCodePrefix(site);
                    var siteNameInStore = await GetSiteNameAsync(siteCode);
                    if (string.Compare(site, siteNameInStore, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        _logger.LogWarning($"Site name in web config {site} isn't having same site name in database {siteNameInStore}");
                    }

                    var lastedDate = await GetLastedImportDate(siteCode);
                    dates.Add(lastedDate);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ErrorMessageHelper.GenerateMessage(ex));
                }
            }

            if (!dates.Any())
            {
                throw new InvalidOperationException($"Can't find lasted importing member date");
            }

            dates.Sort();
            return dates.First();
        }

        private async Task HandleFetchDataAsync(IEnumerable<(DateTime FromDate, DateTime ToDate)> dateRanges)
        {
            foreach (var range in dateRanges)
            {
                int pageIndex = 0;
                while (true)
                {
                    var data = await _licenseeApi.GetAffCustomerAsync(range.FromDate, range.ToDate, pageIndex, BATCH_COUNT);
                    if (!data.Any())
                    {
                        break;
                    }

                    var insertMemberCommands = GetImportingData(data);
                    foreach (var command in insertMemberCommands)
                    {
                        if (SupportedSites.Any(supportSiteCode => string.Compare(supportSiteCode, command.SiteName, StringComparison.OrdinalIgnoreCase) == 0))
                        {
                            await InsertMembers(command, range.FromDate, range.ToDate);
                        }
                        else
                        {
                            _logger.LogWarning($"No Import Setting for Prefix: {command.SettingPrefix} Site: {command.SiteName}, Imported Data  from {range.FromDate} to {range.ToDate}");
                        }
                    }

                    pageIndex++;
                    if (data.Count() < BATCH_COUNT)
                    {
                        break;
                    }
                }

                await SaveInsertAffMemberTimeTrackerAsync(range.ToDate);
            }
        }

        private async Task SaveInsertAffMemberTimeTrackerAsync(DateTime toDate)
        {
            foreach (var site in SupportedSites)
            {
                var sitePrefixProvider = _licenseeSiteSetting.GetCodePrefix(site);

                var param = new InsertAffMemberTimeTrackerCommand(sitePrefixProvider, _fetchServiceSetting.DataSource)
                {
                    LastImportedTime = toDate,
                    ErrCode = SqlResponseCode.Success,
                    ErrMsg = string.Empty
                };

                await _importerRepository.InsertAffMemberTimeTrackerAsync(param);
                if (param.ErrCode != SqlResponseCode.Success)
                {
                    throw new ImporterException(param.ErrCode, param.ErrMsg);
                }
            }
        }

        private async Task InsertMembers(InsertMemberInfoCommand command, DateTime fromDate, DateTime toDate)
        {
            try
            {
                await _importerRepository.ImportMemberAsync(command);
                _logger.LogInformation($"Prefix: {command.SettingPrefix} Site: {command.SiteName}, Imported Data from {fromDate} to {toDate} by SP {command.GetSettingKey()}: {command.XmlData}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Prefix: {command.SettingPrefix} Site: {command.SiteName} {ex.Message} \n {command.XmlData}");
                throw;
            }
        }

        private IEnumerable<InsertMemberInfoCommand> GetImportingData(IEnumerable<AffCustomerModel> customerData)
        {
            var commandList = new List<InsertMemberInfoCommand>();
            var groupCustomers = customerData.GroupBy(c => c.SiteName);

            foreach (var group in groupCustomers)
            {
                var importData = GenerateImportedDataBySiteName(group.Key, group.ToList());

                commandList.Add(importData);
            }

            return commandList;
        }

        private InsertMemberInfoCommand GenerateImportedDataBySiteName(string siteName, List<AffCustomerModel> customers)
        {
            var prefixSettingCode = _licenseeSiteSetting.GetCodePrefix(siteName);
            var xml = BuildXml(customers);

            return new InsertMemberInfoCommand(prefixSettingCode)
            {
                SettingPrefix = prefixSettingCode,
                SiteName = siteName,
                XmlData = xml
            };
        }

        private static string BuildXml(List<AffCustomerModel> customers)
        {
            var customerXml = string.Join(
                    string.Empty,
                    customers.Select(customer =>
                    {
                        var builder = new StringBuilder();
                        builder.AppendJoin(" ", "<row"
                            , $"custid=\"{HttpUtility.HtmlEncode(customer?.CustId)}\""
                            , $"username=\"{HttpUtility.HtmlEncode(customer?.UserName)}\""
                            , $"currencyid=\"{HttpUtility.HtmlEncode(customer?.CurrencyId)}\""
                            , $"actstatus=\"{HttpUtility.HtmlEncode(customer?.ActStatus)}\""
                            , $"affid=\"{HttpUtility.HtmlEncode(customer?.AffId)}\""
                            , $"regtime=\"{HttpUtility.HtmlEncode(customer?.RegTime)}\""
                            , $"firstdepositdate=\"{customer?.FirstDepositDate}\""
                            , $"status=\"{HttpUtility.HtmlEncode(customer?.Status)}\""
                            , $"istest=\"{HttpUtility.HtmlEncode(customer?.IsTest)}\""
                            , $"affcode=\"{HttpUtility.HtmlEncode(customer?.AffCode)}\""
                            , $"affmedialinkid=\"{HttpUtility.HtmlEncode(customer?.AffMediaLinkId)}\""
                            , "/>");
                        return builder.ToString();
                    }));

            return $"<root>{customerXml}</root>";
        }

        private DateTime GetAdjustmentTime(DateTime fromDate)
            => fromDate.AddMinutes(_fetchServiceSetting.AdjustmentValue);

        private async Task<string> GetSiteNameAsync(string settingPrefixCode)
        {
            return await _importerRepository.GetSiteNameAsync(new SiteNameInfo(settingPrefixCode));
        }

        private async Task<DateTime> GetLastedImportDate(string settingPrefixCode)
        {
            return await _importerRepository.GetLastedMemberImportedTimeAsync(new MemberImportedTimeInfo(settingPrefixCode));
        }

        private TimeSpan GetApiMaxTimeRange()
            => TimeSpan.FromMinutes(_fetchServiceSetting.MaxTimeRange);
    }
}
