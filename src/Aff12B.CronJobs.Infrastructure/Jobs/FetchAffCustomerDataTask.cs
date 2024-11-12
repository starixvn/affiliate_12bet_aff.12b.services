using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Helpers;
using Aff12B.CronJobs.Infrastructure.Jobs.Interfaces;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Web;

namespace Aff12B.CronJobs.Infrastructure.Jobs
{
    public interface IFetchAffCustomerDataTask : IFetchDataTask
    {
        Task FetchDataAsync(DateTime fromDate);
    }

    public sealed class FetchAffCustomerDataTask : IFetchAffCustomerDataTask
    {
        private readonly IAffMemberRepository _importerRepository;
        private readonly ILicenseeApi _licenseeApi;
        private readonly ILogger _logger;
        private readonly LicenseeSiteSetting _setting;

        private IEnumerable<string> SupportedSites => _setting?.SupportedSites;


        public FetchAffCustomerDataTask(ILicenseeApi licenseeApi
            , IAffMemberRepository memberImportRepository
            , ILogger<FetchAffCustomerDataTask> logger
            , IOptions<LicenseeSiteSetting> config)
        {
            _importerRepository = memberImportRepository;
            _licenseeApi = licenseeApi;
            _logger = logger;
            _setting = config.Value ?? throw new NullReferenceException(nameof(_setting));
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public async Task FetchDataAsync()
        {
            if (SupportedSites == null
                || !SupportedSites.Any())
            {
                return;
            }

            var date = await GetMinLastedTimeAsync();
            await FetchDataAsync(date);
        }

        public async Task FetchDataAsync(DateTime regTime)
        {
            var customerData = await _licenseeApi.GetAffCustomerAsync(regTime);

            if (customerData == null
                || !customerData.Any())
            {
                _logger.LogInformation($"No Data on date {regTime}");

                return;
            }

            var insertMemberCommands = GetImportingData(customerData);
            foreach (var command in insertMemberCommands)
            {

                if (SupportedSites.Any(supportSiteCode => string.Compare(supportSiteCode, command.SiteName, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    await InsertMembers(regTime, command);
                }
                else
                {
                    _logger.LogInformation($"No Import Setting for Prefix: {command.SettingPrefix} Site: {command.SiteName}, Imported Data {regTime}");
                }
            }
        }

        private async Task InsertMembers(DateTime regTime, InsertMemberInfoCommand command)
        {
            try
            {
                await _importerRepository.ImportMemberAsync(command);
                _logger.LogInformation($"Prefix: {command.SettingPrefix} Site: {command.SiteName}, Imported Data {regTime} by SP {command.GetSettingKey()}: {command.XmlData}");
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
            var prefixSettingCode = _setting.GetCodePrefix(siteName);
            var xml = BuildXml(customers);

            return new InsertMemberInfoCommand(prefixSettingCode)
            {
                SettingPrefix = prefixSettingCode,
                SiteName = siteName,
                XmlData = xml
            };
        }

        private string BuildXml(List<AffCustomerModel> customers)
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
                            , "/>");
                        return builder.ToString();
                    }));

            return $"<root>{customerXml}</root>";
        }

        private async Task<DateTime> GetMinLastedTimeAsync()
        {
            var dates = new List<DateTime>();
            foreach (var site in SupportedSites)
            {
                try
                {
                    var siteCode = _setting.GetCodePrefix(site);
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

        private async Task<DateTime> GetLastedImportDate(string settingPrefixCode)
        {
            return await _importerRepository.GetLastedMemberImportedTimeAsync(new MemberImportedTimeInfo(settingPrefixCode));
        }

        private async Task<string> GetSiteNameAsync(string settingPrefixCode)
        {
            return await _importerRepository.GetSiteNameAsync(new SiteNameInfo(settingPrefixCode));
        }
    }
}