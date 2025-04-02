using MauiDevEnv;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;


var dni = await DotnetTools.GetDotNetInfo();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMcpServer()
	.WithStdioServerTransport()
	.WithToolsFromAssembly();

builder.Logging.ClearProviders();

var app = builder.Build();

await app.RunAsync();
