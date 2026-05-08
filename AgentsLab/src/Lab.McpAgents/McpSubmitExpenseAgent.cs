using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace McpAgents;

public class McpSubmitExpenseAgent(
    IChatClientBuilder chatClientBuilder
) : IRunnableLab
{
    public List<TagType> GetTags() => [
        TagType.Conversation,
        TagType.ChatHistory,
        TagType.Tools,
        TagType.Mcp];

    public List<string> GetLabDescriptions() => [
        "Demonstrate an agent calling a MCP service to create an expense.",
        "The MCP server provides tools and resources to the agent.",
        "",
        "Architecture:",
        "SubmitExpenseAgent -> FinanceMCPClient -> FinanceMCPServer -> FinanceExpenseAPI",
        "",
        "Components:",
        "* SubmitExpenseAgent - implemented in this project, runs the agent. It is the MCP host.",
        "* FinanceMCPClient   - implemented in this project, connects to the finance MCP server",
        "",
        "* FinanceMCPServer   - MCP server for the Finance business domain, uses HTTP transport",
        "* FinanceExpenseAPI  - REST API that executes the business logic for expenses (it implements the tools GetExpenses and SubmitExpense)",
        "* FinanceTaxAPI      - REST API that executes the business logic for taxes  (it implements the tool GetTaxDocuments)",
        "",
        "Make sure the AgentsLab.AppHost is running, as this lab requires the MCP server and the expense REST API.",
    ];

    public string GetUserInput() => "Create an expense for a meal for employee Id 2344 with value of CHF 34.";

    public async Task<string?> RunLabAsync(string userInput)
    {
        var financeMcpTransport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri("https://localhost:7271/mcp"),
            TransportMode = HttpTransportMode.StreamableHttp
        });

        await using var financeMcpClient = await McpClient.CreateAsync(financeMcpTransport);
        var financeTools = await financeMcpClient.ListToolsAsync();

        Console.WriteLine($"Tools available for the agent: {string.Join(", ", financeTools)}");

        var policyResult = await financeMcpClient.ReadResourceAsync("finance://expenses/policy");
        var fieldsResult = await financeMcpClient.ReadResourceAsync("finance://expenses/required-fields");

        var policy = policyResult.Contents.OfType<TextResourceContents>().FirstOrDefault()?.Text ?? "";
        var requiredFields = fieldsResult.Contents.OfType<TextResourceContents>().FirstOrDefault()?.Text ?? "";

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "ExpenseAgent",
            ChatOptions = new ChatOptions
            {
                Instructions = $"""
                               You are an expense assistant.
                               {requiredFields}
                               {policy}
                               """,
                Tools = [..financeTools]
            }
        };

        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaQwenVl4B);

        var agent = new ChatClientAgent(chatClient, agentOptions);

        var session = await agent.CreateSessionAsync();

        Console.WriteLine(userInput);

        var response = await agent.RunAsync(userInput, session);

        Console.WriteLine($"  {response.Text}");

        return null;
    }
}
