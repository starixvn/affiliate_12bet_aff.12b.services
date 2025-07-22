using Aff12B.CronJobs.Host;
using Aff12B.CronJobs.Infrastructure.Apis;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;
// Add config to the container
builder.Configuration.SetBasePath(env.ContentRootPath);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false);
builder.Configuration.AddEnvironmentVariables();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.WebHost.UseSentry();

// Add services to the container.
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
app.MapGet("/", () => "Hello, Welcome to 12B CronJobs!");
app.MapGet("/test/accessapi", () =>
{
    var api = app.Services.GetRequiredService<ILicenseeApi>();

    return api.CheckConnection();
});

// Configure the HTTP request pipeline.
startup.Configure(app, app.Environment);

app.Run();