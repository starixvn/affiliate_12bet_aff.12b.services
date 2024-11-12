using Aff12B.Ext.API.Core.Services;
using Aff12B.Ext.API.Infastructure.ConfigurationModel;
using Aff12B.Ext.API.Infastructure.Extentions;
using Microsoft.Extensions.Options;

namespace Aff12B.Ext.API.Infastructure.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IEnumerable<User> _users;

        public UserManagementService(IOptions<List<User>> userListAccessor)
        {
            _users = userListAccessor.Value;
        }

        public bool IsValidUser(string username, string password)
            => _users.Any(x => x.UserName.Equals(username)
            && x.Password.Equals(password.Sha256()));

    }
}
