var builder = DistributedApplication.CreateBuilder(args);

// Finance domain
builder.AddProject<Projects.LabApi_FinanceExpenseApi>("finance-api-expense")
    .WithHttpsEndpoint(port: 7261, name: "https")
    .WithHttpEndpoint(port: 5261, name: "http");

builder.AddProject<Projects.LabApi_FinanceTaxApi>("finance-api-tax")
    .WithHttpsEndpoint(port: 7262, name: "https")
    .WithHttpEndpoint(port: 5262, name: "http");

builder.AddProject<Projects.LabMcp_FinanceMcpServer>("finance-mcp-server")
    .WithHttpsEndpoint(port: 7271, name: "https")
    .WithHttpEndpoint(port: 5271, name: "http");

// IT Support domain
builder.AddProject<Projects.LabApi_ItSupportApi>("it-support-api")
    .WithHttpsEndpoint(port: 7263, name: "https")
    .WithHttpEndpoint(port: 5263, name: "http");

builder.AddProject<Projects.LabMcp_ItSupportMcpServer>("it-support-mcp-server")
    .WithHttpsEndpoint(port: 7272, name: "https")
    .WithHttpEndpoint(port: 5272, name: "http");

builder.Build().Run();