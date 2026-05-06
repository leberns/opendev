using McpServerExpense.Resources;
using McpServerExpense.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<ExpenseTools>();

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<ExpenseTools>()
    .WithResources<ExpenseResources>();

var app = builder.Build();

app.MapMcp("/mcp");

app.Run("http://localhost:5100");
