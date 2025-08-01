using Aff12B.Common.DataDomain.Models;
using Aff12B.Common.Infrastructure.Helpers;
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
    public class BaseAffCustomerDataTask
    {
        protected const int BATCH_COUNT = 2000;
        protected virtual bool IsNeedRealData => false;

        protected readonly IAffMemberRepository _importerRepository;
        protected readonly ILicenseeApi _licenseeApi;
        protected readonly ILogger _logger;
        protected readonly FetchServiceSetting _fetchServiceSetting;
        protected readonly LicenseeSiteSetting _licenseeSiteSetting;

        protected IEnumerable<string> SupportedSites => _licenseeSiteSetting?.SupportedSites;

        protected BaseAffCustomerDataTask(ILicenseeApi licenseeApi
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

            if (IsNeedRealData)
            {
                await HandleFetchRealDataAsync(dateRanges, apiServerNow);
            }
        }

        private async Task<DateTime> GetMinLastedTimeAsync()
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

                    var lastedDate = await GetLastedImportDate(siteCode, _fetchServiceSetting.DataSource);
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
            return dates[0];
        }

        private async Task HandleFetchDataAsync(IEnumerable<(DateTime FromDate, DateTime ToDate)> dateRanges)
        {
            foreach (var range in dateRanges)
            {
                await HandleImportDataAsync(range.FromDate, range.ToDate);

                await SaveInsertAffMemberTimeTrackerAsync(range.ToDate);
            }
        }

        private async Task HandleFetchRealDataAsync(IEnumerable<(DateTime FromDate, DateTime ToDate)> dateRanges, DateTime apiServerNow)
        {
            var lastDateRange = dateRanges.LastOrDefault();

            if (lastDateRange.ToDate != default(DateTime))
            {
                await HandleImportDataAsync(lastDateRange.ToDate, apiServerNow.AddSeconds(-5));
            }
        }

        private async Task HandleImportDataAsync(DateTime fromDate, DateTime toDate)
        {
            int pageIndex = 0;
            bool hasMoreData = true;

            while (hasMoreData)
            {
                var data = await _licenseeApi.GetAffCustomerAsync(fromDate, toDate, pageIndex);
                if (!data.Any())
                {
                    hasMoreData = false;
                    continue;
                }

                var insertMemberCommands = GetImportingData(data);
                foreach (var command in insertMemberCommands)
                {
                    if (SupportedSites.Any(supportSiteCode => string.Compare(supportSiteCode, command.SiteName, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        await InsertMembers(command, fromDate, toDate);
                    }
                    else
                    {
                        _logger.LogWarning($"No Import Setting for Prefix: {command.SettingPrefix} Site: {command.SiteName}, Imported Data  from {fromDate} to {toDate}");
                    }
                }

                pageIndex++;
                if (data.Count() < BATCH_COUNT)
                {
                    hasMoreData = false;
                }
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

        private List<InsertMemberInfoCommand> GetImportingData(IEnumerable<AffCustomerModel> customerData)
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
                customers.Select(BuildCustomerRowXml)
            );

            return $"<root>{customerXml}</root>";
        }

        private static string BuildCustomerRowXml(AffCustomerModel customer)
        {
            var safeCustomer = customer ?? new AffCustomerModel();
            var builder = new StringBuilder();
            builder.Append("<row");
            AppendXmlAttribute(builder, "custid", safeCustomer.CustId);
            AppendXmlAttribute(builder, "username", safeCustomer.UserName);
            AppendXmlAttribute(builder, "currencyid", safeCustomer.CurrencyId);
            AppendXmlAttribute(builder, "actstatus", safeCustomer.ActStatus);
            AppendXmlAttribute(builder, "affid", safeCustomer.AffId);
            AppendXmlAttribute(builder, "regtime", safeCustomer.RegTime);
            AppendXmlAttribute(builder, "firstdepositdate", safeCustomer.FirstDepositDate);
            AppendXmlAttribute(builder, "status", safeCustomer.Status);
            AppendXmlAttribute(builder, "istest", safeCustomer.IsTest);
            AppendXmlAttribute(builder, "affcode", safeCustomer.AffCode);
            AppendXmlAttribute(builder, "affmedialinkid", safeCustomer.AffMediaLinkId);
            AppendXmlAttribute(builder, "clickid", safeCustomer.ClickId);
            builder.Append(" />");
            return builder.ToString();
        }

        private static void AppendXmlAttribute(StringBuilder builder, string attributeName, object value)
        {
            builder.Append(' ')
                   .Append(attributeName)
                   .Append("=\"")
                   .Append(HttpUtility.HtmlEncode(value))
                   .Append("\"");
        }

        private DateTime GetAdjustmentTime(DateTime fromDate)
            => fromDate.AddMinutes(_fetchServiceSetting.AdjustmentValue);

        private async Task<string> GetSiteNameAsync(string settingPrefixCode)
        {
            return await _importerRepository.GetSiteNameAsync(new SiteNameInfo(settingPrefixCode));
        }

        private async Task<DateTime> GetLastedImportDate(string settingPrefixCode, string sName)
        {
            return await _importerRepository.GetLastedMemberImportedTimeAsync(new MemberImportedTimeInfo(settingPrefixCode, sName));
        }

        private TimeSpan GetApiMaxTimeRange()
            => TimeSpan.FromMinutes(_fetchServiceSetting.MaxTimeRange);
    }
}
