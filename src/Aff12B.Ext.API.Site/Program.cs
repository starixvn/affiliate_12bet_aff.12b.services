using Aff12B.Ext.API.Site;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;
// Add config to the container
builder.Configuration.SetBasePath(env.ContentRootPath);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
startup.Configure(app, app.Environment);

app.Run();