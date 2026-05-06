using System.ComponentModel;
using System.Text;
using ModelContextProtocol.Server;

namespace McpServerExpense.Tools;

/// <summary>
/// Sample MCP tools for demonstration purposes.
/// These tools can be invoked by MCP clients to perform various operations.
/// </summary>
internal class ExpenseTools(HttpClient httpClient)
{
    [McpServerTool]
    [Description("Creates a expense request.")]
    public async Task<string> CreateExpense(
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
}