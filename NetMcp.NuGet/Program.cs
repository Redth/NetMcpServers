using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using NetMcp.NuGet;

var builder = Host.CreateApplicationBuilder(args);

//var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMcpServer()
    .WithListResourceTemplatesHandler(NuGetResources.ListResourceTemplatesHandler)
	.WithListResourcesHandler(NuGetResources.ListResourcesHandler)
    .WithReadResourceHandler(NuGetResources.ReadResourceHandler)
    //.WithHttpListenerSseServerTransport()
	.WithStdioServerTransport()
    .WithTools();

builder.Logging.ClearProviders();

var app = builder.Build();

await app.RunAsync();
