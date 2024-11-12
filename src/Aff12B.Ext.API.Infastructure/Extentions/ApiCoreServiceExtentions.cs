using Aff12B.Ext.API.Core.Services;
using Aff12B.Ext.API.Infastructure.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Aff12B.Ext.API.Infastructure.Extentions
{
    public static class ApiCoreServiceExtentions
    {
        public static IServiceCollection AddApiCoreService(this IServiceCollection service)
        {
            service.AddSingleton<IDynamicRepository, DynamicRepository>();

            service.AddScoped<IAuthService, AuthService>();
            service.AddScoped<IUserManagementService, UserManagementService>();
            return service;
        }
    }
}
