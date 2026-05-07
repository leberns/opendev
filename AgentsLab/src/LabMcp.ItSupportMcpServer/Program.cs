using ItSupportMCPServer.Resources;
using ItSupportMCPServer.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient<ItSupportTools>();

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithResources<ItSupportResources>()
    .WithTools<ItSupportTools>();

var app = builder.Build();

app.MapMcp("/mcp");

app.Run();
