using Azure.Identity;
using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Contracts.Settings;
using Microsoft.Agents.AI;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.AI;

namespace MultiTurnConversation;

public class MeetingHistoryCosmosDbCont(
    AiSettings aiSettings,
    IChatClientBuilder chatClientBuilder
    ) : IRunnableLab
{
    public List<TagType> GetTags() => [TagType.Conversation, TagType.ChatHistory, TagType.Azure, TagType.CosmosDb];

    public List<string> GetLabDescriptions() => [
        "Demonstrate the agent continuing a conversation from a previous chat stored in Azure Cosmos DB.",
        "The conversation the agent continues has to be started on the same day, check conversationId below.",
        "This lab requires the Azure Cosmos DB, check in README.md how to provision the infrastructure.",
        "Suggestion: check the collection in Cosmos DB, how the conversation has changed there.",
    ];

    public string GetUserInput() => "Who was in the meeting today? What was the meeting about? Was something decided?";

    public async Task<string?> RunLabAsync(string userInput)
    {
        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaLlama);

        // to create the Cosmos DB container on Azure, verify how to provision the infrastructure in the README.md file
        var cosmosClient = new CosmosClient(
            accountEndpoint: aiSettings.AgentsDbAccountEndpoint,
            tokenCredential: new DefaultAzureCredential() // current user
        );

        var chatHistoryProvider = new CosmosChatHistoryProvider(
            cosmosClient: cosmosClient,
            databaseId: "agentsdb",
            containerId: "history",
            stateInitializer: _ => new CosmosChatHistoryProvider.State(
                conversationId: $"conversation-{DateTime.UtcNow:yyyy-MM-dd}",
                tenantId: "tenant-1",
                userId: "user-1"
            )
        );

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "MeetingAgent",
            ChatHistoryProvider = chatHistoryProvider,
            ChatOptions = new ChatOptions
            {
                Instructions = "You are a friendly assistant. " +
                               "Answer the questions when asked, otherwise just observe the conversation. " +
                               "Keep answers very brief."
            }
        };

        var agent = new ChatClientAgent(chatClient, agentOptions);

        var session = await agent.CreateSessionAsync();

        var response1 = await agent.RunAsync(userInput, session);

        Console.WriteLine($"  {response1.Text}");

        var serialized = await agent.SerializeSessionAsync(session);

        Console.WriteLine("Session:");
        Console.WriteLine($"  {serialized}");

        return null;
    }
}
