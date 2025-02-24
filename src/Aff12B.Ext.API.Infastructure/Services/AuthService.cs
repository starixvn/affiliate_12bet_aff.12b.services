using Aff12B.Ext.API.Core.Models;
using Aff12B.Ext.API.Core.Params;
using Aff12B.Ext.API.Core.Services;
using Aff12B.Ext.API.Infastructure.ConfigurationModel;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Aff12B.Ext.API.Infastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserManagementService _userManagementService;
        private readonly TokenManagement _tokenManagement;


        public AuthService(IUserManagementService service, IOptions<TokenManagement> tokenManagementAccessor)
        {
            _userManagementService = service;
            _tokenManagement = tokenManagementAccessor.Value;

        }

        public Task<Tuple<bool, Token>> IsAuthenticated(TokenRequest request)
        {
            var token = new Token();
            var userName = request.Username;

            if (!_userManagementService.IsValidUser(userName, request.Password))
            {
                return Task.FromResult(Tuple.Create(false, token));
            }


            var claim = new[]
            {
                new Claim(ClaimTypes.Name, userName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var encryptCredenction = new EncryptingCredentials(key, JwtConstants.DirectKeyUseAlg,
                SecurityAlgorithms.Aes256CbcHmacSha512);
            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.CreateJwtSecurityToken(_tokenManagement.Issuer,
                  _tokenManagement.Audience,
                  new ClaimsIdentity(claim),
                  DateTime.Now,
                  DateTime.Now.AddMinutes(_tokenManagement.AccessExpiration),
                  null,
                  credentials,
                  encryptCredenction
                  );

            token.AccessToken = handler.WriteToken(jwtToken);
            token.TokenType = handler.TokenType.Name;
            token.ExpiresIn = _tokenManagement.AccessExpiration;

            return Task.FromResult(Tuple.Create(true, token));

        }
    }
}
