using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace McpAgents;

public class McpServicesAgent(
    IChatClientBuilder chatClientBuilder
) : IRunnableLab
{
    public List<TagType> GetTags() => [
        TagType.Conversation,
        TagType.ChatHistory,
        TagType.Tools,
        TagType.Mcp,
        TagType.UserInteraction];

    public List<string> GetLabDescriptions() => [
        "Demonstrate an agent that can perform several services calling the proper MCP servers.",
        "The MCP servers provide tools and resources to the agent.",
        "",
        "Architecture:",
        "ServicesAgent -> FinanceMCPClient -> FinanceMCPServer -> FinanceExpenseAPI",
        "                                                         FinanceTaxAPI",
        "                 ItSupportMCPClient -> ItSupportMCPServer -> ItSupportAPI",
        "",
        "Components:",
        "* ServicesAgent      - implemented in this project, runs the agent. It is the MCP host.",
        "* FinanceMCPClient   - implemented in this project, connects to the finance MCP server",
        "* ItSupportMCPClient - implemented in this project, connects to the IT support MCP server",
        "",
        "* FinanceMCPServer   - MCP server for the Finance business domain, uses HTTP transport",
        "* FinanceExpenseAPI  - REST API that executes the business logic for expenses (it implements the tools GetExpenses and SubmitExpense)",
        "* FinanceTaxAPI      - REST API that executes the business logic for taxes  (it implements the tool GetTaxDocuments)",
        "",
        "* ItSupportMCPServer - MCP server for the IT support business domain, uses HTTP transport",
        "* ItSupportAPI       - REST API that executes the logic for IT support (it implements the tool CreateItSupportTicket)",
        "",
        "Make sure the AgentsLab.AppHost is running, as this lab requires the MCP servers and REST APIs.",
        "",
        "The input is incomplete, so the agent asks the user for more information (the cost of the meal to be expensed).",
    ];

    public string GetUserInput() => "What are my expenses so far, add an expense for a meal for employee Id 2344";

    public async Task<string?> RunLabAsync(string userInput)
    {
        var financeMcpTransport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri("https://localhost:7271/mcp"),
            TransportMode = HttpTransportMode.StreamableHttp
        });

        await using var financeMcpClient = await McpClient.CreateAsync(financeMcpTransport);
        var financeTools = await financeMcpClient.ListToolsAsync();

        Console.WriteLine($"Finance tools: {string.Join(", ", financeTools)}");

        var itSupportMcpTransport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri("https://localhost:7272/mcp"),
            TransportMode = HttpTransportMode.StreamableHttp
        });

        await using var itSupportMcpClient = await McpClient.CreateAsync(itSupportMcpTransport);
        var itSupportTools = await itSupportMcpClient.ListToolsAsync();

        Console.WriteLine($"IT Support tools: {string.Join(", ", itSupportTools)}");

        var policyResult = await financeMcpClient.ReadResourceAsync("finance://expenses/policy");
        var expenseFieldsResult = await financeMcpClient.ReadResourceAsync("finance://expenses/required-fields");
        var taxFieldsResult = await financeMcpClient.ReadResourceAsync("finance://tax-documents/required-fields");

        var policy = policyResult.Contents.OfType<TextResourceContents>().FirstOrDefault()?.Text ?? "";
        var expenseFields = expenseFieldsResult.Contents.OfType<TextResourceContents>().FirstOrDefault()?.Text ?? "";
        var taxFields = taxFieldsResult.Contents.OfType<TextResourceContents>().FirstOrDefault()?.Text ?? "";

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "ServicesAgent",
            ChatOptions = new ChatOptions
            {
                Instructions = $"""
                               You are a services assistant that handles finance and IT support requests.
                               {expenseFields}
                               {taxFields}
                               {policy}
                               """,
                Tools = [..financeTools, ..itSupportTools]
            }
        };

        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaQwenVl4B);

        var agent = new ChatClientAgent(chatClient, agentOptions);

        var session = await agent.CreateSessionAsync();

        Console.WriteLine(userInput);

        var response = await agent.RunAsync(userInput, session);

        Console.WriteLine($"{response.Text}");

        while (true)
        {
            Console.Write("User follow-up input (press Enter to exit): ");

            var followUp = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(followUp))
            {
                break;
            }

            response = await agent.RunAsync(followUp, session);
            Console.WriteLine($"{response.Text}");
        }

        return null;
    }
}
