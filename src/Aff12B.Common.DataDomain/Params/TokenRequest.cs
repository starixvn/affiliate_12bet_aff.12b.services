
namespace Aff12B.Common.DataDomain.Params
{
    public class TokenRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public int CacheTimeOut { get; set; } = 20;
    }
}
