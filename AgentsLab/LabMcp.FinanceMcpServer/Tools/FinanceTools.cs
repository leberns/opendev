using System.ComponentModel;
using System.Text;
using ModelContextProtocol.Server;

namespace FinanceMCPServer.Tools;

/// <summary>
/// Register the finance tools
/// </summary>
internal class FinanceTools(HttpClient httpClient)
{
    [McpServerTool]
    [Description("Get the employee's expenses.")]
    public async Task<string> GetExpenses(
        [Description("Employee Id")] int employeeId)
    {
        var url = $"https://localhost:7261/api/expenses/{employeeId}";

        var response = await httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    [McpServerTool]
    [Description("Submit an expense request.")]
    public async Task<string> SubmitExpense(
        [Description("Employee Id")] string employeeId,
        [Description("Description")] string description,
        [Description("Expense amount")] decimal amount,
        [Description("Expense currency")] string currency)
    {
        const string url = "https://localhost:7261/api/expenses";

        var requestBody = $"{{\"employeeId\": \"{employeeId}\", \"description\": \"{description}\", \"amount\": {amount}, \"currency\": \"{currency}\"}}";

        var response = await httpClient.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        return "created";
    }

    [McpServerTool]
    [Description("Get the employee's tax documents.")]
    public async Task<string> GetTaxDocuments(
        [Description("Employee Id")] string employeeId,
        [Description("Begin Year")] int yearBegin,
        [Description("End Year")] int yearEnd)
    {
        var url = $"https://localhost:7262/api/tax-documents/{employeeId}/{yearBegin}/{yearEnd}";

        var response = await httpClient.PostAsync(url, null);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}