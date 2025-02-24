namespace Aff12B.Ext.API.Core.Services
{
    public interface IUserManagementService
    {
        bool IsValidUser(string username, string password);
    }
}
