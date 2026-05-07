using FinanceMCPServer.Resources;
using FinanceMCPServer.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient<FinanceTools>();

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<FinanceTools>()
    .WithResources<FinanceResources>();

var app = builder.Build();

app.MapMcp("/mcp");

app.Run();