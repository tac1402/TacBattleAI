
using Tac.Sql;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<SqlService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Настраиваем для работы как Windows Service
builder.Services.AddWindowsService(options =>
{
	options.ServiceName = "TacSqlService";
});

var host = builder.Build();
await host.RunAsync();