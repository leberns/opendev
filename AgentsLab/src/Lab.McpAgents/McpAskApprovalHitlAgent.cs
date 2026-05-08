using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace McpAgents;

public class McpAskApprovalHitlAgent(
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
        "Demonstrate an agent calling a MCP service to create IT support, the user is asked to confirm before creating the ticket.",
        "HITL = Human in the loop, the user is asked to confirm before the agent creates the ticket.",
        "",
        "Make sure the AgentsLab.AppHost is running, as this lab requires the MCP server and the IT support REST API.",
    ];

    public string GetUserInput() => "Create a ticket for IT support to provision a VM in the cloud.";

    public async Task<string?> RunLabAsync(string userInput)
    {
        var itSupportMcpTransport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri("https://localhost:7272/mcp"),
            TransportMode = HttpTransportMode.StreamableHttp
        });

        await using var itSupportClient = await McpClient.CreateAsync(itSupportMcpTransport);
        var itSupportTools = await itSupportClient.ListToolsAsync();

        Console.WriteLine($"Tools available for the agent: {string.Join(", ", itSupportTools)}");

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "ITSupportAgent",
            ChatOptions = new ChatOptions
            {
                Instructions = $"""
                               You are an IT support assistant.
                               """,
                Tools = [..itSupportTools]
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
