// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Aff12B.Ext.API.Site.Controllers
{
    using System.Text;
    using System.Text.Json;
    using Common.DataDomain.Api;
    using Common.DataDomain.Models;
    using Common.DataDomain.Params;
    using Fanex.Data.Repository;
    using Infastructure.Command;
    using Infastructure.Criteria;
    using Infastructure.Params;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private const int _Marketing_Cust_ID = -1;
        private readonly IDynamicRepository _dynamicRepository;
        private readonly ILogger _logger;

        public CustomerController(IDynamicRepository dynamicRepository, ILogger<CustomerController> logger)
        {
            _dynamicRepository = dynamicRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<AffCustomerModel>> GetAffCustomers(DateTime fromDate, DateTime toDate, int pageIndex, int pageSize = 2000)
            => await _dynamicRepository.FetchAsync<AffCustomerModel>(new GetAffCustomersCriteria(fromDate, toDate, pageIndex, pageSize));

        [HttpGet]
        public async Task<IEnumerable<CustomerModel>> GetCustomers(DateTime fromDate, DateTime toDate, int pageIndex, int pageSize = 2000)
            => await _dynamicRepository.FetchAsync<CustomerModel>(new GetCustomersCriteria(fromDate, toDate, pageIndex, pageSize));

        [HttpGet]
        public async Task<CustomerModel> GetLoginCustomerInfo(string userName, int siteId)
        {
            CustomerModel cust = null;
            if (!string.IsNullOrWhiteSpace(userName) && !HasNonASCIIChars(userName))
            {
                cust = await _dynamicRepository.GetAsync<CustomerModel>(new GetLoginCustInfoCriteria { UserName = userName, SiteId = siteId });
            }
            return cust;
        }

        [HttpPost]
        public async Task<decimal> UpdateCustomerWallet(PaymentRequestParam payment)
        {
            _logger.LogInformation($"Recevice update wallet request\n" +
                $"Param: {JsonSerializer.Serialize(payment)}\n" +
                $"RequestIp: ${GetIpAddress()}");
            var criteria = new UpdateCustomerWalletCommand
            {
                Amount = payment.Amount,
                AfterAmount = 0,
                CustId = payment.CustId,
                Memo = payment.Memo,
                RefTransId = payment.RefTransId,
                SiteId = payment.SiteId,
                Type = payment.Type,
                UserName = payment.UserName
            };

            await _dynamicRepository.ExecuteAsync(criteria);
            _logger.LogInformation($"AfterAmount: {criteria.AfterAmount}");
            return criteria.AfterAmount;
        }

        [HttpPost]
        public async Task<ResultInfo> UnlockUser(int[] userIds)
        {
            try
            {
                var json = BuildJson(userIds);
                await _dynamicRepository.ExecuteAsync(new UnlockUsersCommand { CustIdList = json });
                return new ResultInfo
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new ResultInfo
                {
                    IsSuccess = false,
                    ErrMsg = ex.Message
                };
            }
        }

        [HttpPost]
        public async Task<ResultInfo> InsertAffiliateInfo(AffiliateParam affiliate)
        {
            try
            {
                await _dynamicRepository.ExecuteAsync(new InsertAffiliateInfoCommand
                {
                    AffiliateId = affiliate.AffiliateId,
                    AffiliateCode = affiliate.AffiliateCode,
                    AffiliateName = affiliate.AffiliateName,
                    CustId = affiliate.CustId == 0 ? _Marketing_Cust_ID : affiliate.CustId,
                    ParentAffiliate = affiliate.ParentAffiliate ?? string.Empty,
                    SiteId = affiliate.SiteId,
                    UserName = affiliate.UserName ?? string.Empty,
                    FirstName = affiliate.FirstName ?? string.Empty,
                    LastName = affiliate.LastName ?? string.Empty,
                    ContactNumber = affiliate.ContactNumber ?? string.Empty,
                    Email = affiliate.Email ?? string.Empty,
                    Status = affiliate.Status ?? string.Empty,
                    DealVerifyFlags = affiliate.DealVerifyFlags
                });
                return new ResultInfo { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new ResultInfo
                {
                    IsSuccess = false,
                    ErrMsg = ex.Message
                };
            }
        }

        [HttpPost]
        public async Task<ResultInfo> InsertIdCode(IdCodeParam param)
        {
            try
            {
                await _dynamicRepository.ExecuteAsync(new InsertIdCodeCommand
                {
                    IdCode = param.IdCode,
                    SiteId = param.SiteId,
                    StatusId = param.StatusId
                });
                return new ResultInfo { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new ResultInfo
                {
                    IsSuccess = false,
                    ErrMsg = ex.Message
                };
            }
        }

        private static string BuildJson(int[] userIds)
            => $"[{string.Join(",", userIds)}]";

        private string GetIpAddress()
            => HttpContext.Connection.RemoteIpAddress.ToString();

        private static bool HasNonASCIIChars(string str)
            => Encoding.UTF8.GetByteCount(str) != str.Length;
    }
}