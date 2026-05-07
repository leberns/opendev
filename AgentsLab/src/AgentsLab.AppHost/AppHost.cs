var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.LabApi_FinanceExpenseApi>("finance-api-expense");
builder.AddProject<Projects.LabApi_FinanceTaxApi>("finance-api-tax");
builder.AddProject<Projects.LabMcp_FinanceMcpServer>("finance-mcp-server");

builder.AddProject<Projects.LabApi_ItSupportApi>("it-support-api");
builder.AddProject<Projects.LabMcp_ItSupportMcpServer>("it-support-mcp-server");

builder.Build().Run();