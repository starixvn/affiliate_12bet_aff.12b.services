using Aff12B.Ext.API.Core.Models;
using Aff12B.Ext.API.Core.Params;

namespace Aff12B.Ext.API.Core.Services
{
    public interface IAuthService
    {
        Task<Tuple<bool, Token>> IsAuthenticated(TokenRequest request);
    }
}
