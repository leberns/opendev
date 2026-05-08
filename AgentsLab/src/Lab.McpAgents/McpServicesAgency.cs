using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace McpAgents;

public class McpServicesAgency(
    IChatClientBuilder chatClientBuilder
) : IRunnableLab
{
    public List<TagType> GetTags() => [
        TagType.Conversation,
        TagType.ChatHistory,
        TagType.Tools,
        TagType.Mcp,
        TagType.MultiAgent,
        TagType.UserInteraction];

    public List<string> GetLabDescriptions() => [
        "Demonstrate an agency with a plan agent that routes requests to specialized finance and IT support agents.",
        "The MCP servers provide tools and resources to the specialized agents.",
        "",
        "Architecture:",
        "PlanAgent +> FinanceAgent -> FinanceMCPClient -> FinanceMCPServer +> FinanceExpenseAPI",
        "          |                                                       +> FinanceTaxAPI",
        "          +> ItSupportAgent -> ItSupportMCPClient -> ItSupportMCPServer -> ItSupportAPI",
        "",
        "Components:",
        "* PlanAgent          - decomposes the user request and delegates to specialized agents",
        "* FinanceAgent       - handles finance requests using the Finance MCP server tools",
        "* ItSupportAgent     - handles IT support requests using the IT Support MCP server tools",
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
    ];

    public string GetUserInput() => "What are my expenses so far, add an expense for a meal for employee Id 2344 with value of CHF 34 and get the tax document for 2023.";

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

        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaQwenVl4B);

        AIAgent financeAgent = new ChatClientAgent(
            chatClient,
            instructions: $"""
                          You are a finance assistant.
                          {expenseFields}
                          {taxFields}
                          {policy}
                          """,
            name: "FinanceAgent",
            tools: [..financeTools]);

        AIAgent itSupportAgent = new ChatClientAgent(
            chatClient,
            instructions: "You are an IT support assistant. Help with IT support requests.",
            name: "ItSupportAgent",
            tools: [..itSupportTools]);

        var financeAgentTool = AIFunctionFactory.Create(
            async (string request) =>
            {
                var response = await financeAgent.RunAsync(request);
                return response.Text;
            },
            "FinanceAgent",
            "Handles finance requests: get expenses, submit expenses, get tax documents.");

        var itSupportAgentTool = AIFunctionFactory.Create(
            async (string request) =>
            {
                var response = await itSupportAgent.RunAsync(request);
                return response.Text;
            },
            "ItSupportAgent",
            "Handles IT support requests.");

        ChatClientAgentOptions planAgentOptions = new()
        {
            Name = "PlanAgent",
            ChatOptions = new ChatOptions
            {
                Instructions = """
                              You are a planning agent. Decompose the user request and delegate:
                              - Finance tasks (expenses, tax documents) to FinanceAgent.
                              - IT support tasks to ItSupportAgent.
                              Synthesize the results into a single response for the user.
                              """,
                Tools = [financeAgentTool, itSupportAgentTool]
            }
        };

        var planAgent = new ChatClientAgent(chatClient, planAgentOptions);

        var session = await planAgent.CreateSessionAsync();

        Console.WriteLine(userInput);

        var response = await planAgent.RunAsync(userInput, session);

        Console.WriteLine($"{response.Text}");

        while (true)
        {
            Console.Write("User follow-up input (press Enter to exit): ");

            var followUp = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(followUp))
            {
                break;
            }

            response = await planAgent.RunAsync(followUp, session);
            Console.WriteLine($"{response.Text}");
        }

        return null;
    }
}
