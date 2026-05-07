using System.ComponentModel;
using System.Text;
using ModelContextProtocol.Server;

namespace ItSupportMCPServer.Tools;

/// <summary>
/// Register the IT support tools
/// </summary>
internal class ItSupportTools(HttpClient httpClient)
{
    [McpServerTool]
    [Description("Create an IT support ticket.")]
    public async Task<string> CreateItSupportTicket(
        [Description("Employee Id")] string employeeId,
        [Description("Description")] string description)
    {
        const string url = "https://localhost:7263/api/it-support";

        var requestBody = $"{{\"employeeId\": \"{employeeId}\", \"description\": \"{description}\"}}";

        var response = await httpClient.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        return "created";
    }
}