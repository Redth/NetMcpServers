using MauiDevEnv;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;


var dni = await DotnetTools.GetDotNetInfo();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMcpServer()
	.WithStdioServerTransport()
	.WithTools();

builder.Logging.ClearProviders();

var app = builder.Build();

await app.RunAsync();
