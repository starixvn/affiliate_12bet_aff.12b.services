using Aff12B.Common.Infrastructure.Middleware;
using Aff12B.Ext.API.Infastructure.ConfigurationModel;
using Aff12B.Ext.API.Infastructure.Extentions;
using Fanex.Data.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Security.Claims;
using System.Text;

namespace Aff12B.Ext.API.Site
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddApiCoreService();
            services.Configure<TokenManagement>(Configuration.GetSection("TokenManagement"));
            services.Configure<List<User>>(Configuration.GetSection("Users"));
            services.AddFanexSql(Configuration);
            ConfigAuthentication(services);
            services.AddSerilog();
            services.AddAuthorizationBuilder()
                .SetDefaultPolicy(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build());

            services.AddSwaggerGen(c =>
            {
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "Aff12B.Ext.API.Site", Version = "v2" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorHandler>();
            if (env.IsDevelopment() || Configuration.GetValue<string>("Environment") == Environments.Development)
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v2/swagger.json", "Aff12B.Ext.API.Site v2"));
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseSerilogRequestLogging(options =>
            {
                options.GetLevel = (ctx, _, ex) =>
                {
                    if (ex != null || ctx.Response.StatusCode > 499)
                    {
                        return LogEventLevel.Error;
                    }
                    return LogEventLevel.Information;
                };
            });

            app.UseRouting();
            app.UseJWTokenMiddleware();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.Map("/", async context =>
                {
                    await context.Response.WriteAsync("Hello, Welcome to 12B Web API!");
                });
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigAuthentication(IServiceCollection services)
        {
            var token = Configuration.GetSection("TokenManagement").Get<TokenManagement>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
                        TokenDecryptionKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
                        ValidIssuer = token.Issuer,
                        ValidAudience = token.Audience,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        RoleClaimType = ClaimsIdentity.DefaultNameClaimType,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                    };
                    x.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Append("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}
