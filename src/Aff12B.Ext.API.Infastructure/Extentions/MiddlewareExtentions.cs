using Aff12B.Ext.API.Infastructure.Middlewares;
using Microsoft.AspNetCore.Builder;


namespace Aff12B.Ext.API.Infastructure.Extentions
{
    public static class MiddlewareExtentions
    {
        public static IApplicationBuilder UseJWTokenMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JWTokenHandler>();
        }
    }
}
