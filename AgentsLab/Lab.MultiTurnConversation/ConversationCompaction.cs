using System.Diagnostics.CodeAnalysis;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Compaction;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Contracts.Settings;
using AgentExtensions;

namespace MultiTurnConversation;

public class ConversationCompaction(
    AiSettings aiSettings,
    IChatClientBuilder chatClientBuilder,
    ILoggerFactory loggerFactory
    ) : IRunnableLab
{
    public List<LabTagType> GetTags() => [LabTagType.Conversation, LabTagType.ChatHistory, LabTagType.Azure, LabTagType.CosmosDb, LabTagType.Compaction];

    public List<string> GetLabDescriptions() => [
        "Demonstrate the conversation compaction.",
        "This feature is useful to reduce tokens usage in long conversations.",
        "It is experimental by the time of this writing.",
        "This lab requires the Azure Cosmos DB, check in README.md how to provision the infrastructure.",
        "Suggestion: check the collection in Cosmos DB, how the conversation is stored and compacted there.",
    ];

    public string GetUserInput() => "Which are the characters in the text?";

    [Experimental("MAAI001")] // remove this once Compaction is no longer experimental
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
                conversationId: $"conversation-play-{DateTime.UtcNow:yyyy-MM-dd}",
                tenantId: "tenant-1",
                userId: "user-1"
            )
        );

        // pure previous messages for demo purposes
        await chatHistoryProvider.ClearMessagesAsync(null);

        var compactionStrategy = new PipelineCompactionStrategy(
            new SummarizationCompactionStrategy(
                chatClient: chatClient,
                trigger: CompactionTriggers.MessagesExceed(1),
                minimumPreservedGroups: 4,
                summarizationPrompt: """
                                     Make a small summary of the conversation:
                                     - Characters involved
                                     - Key topics
                                     - Key concerns
                                     - Key decisions (if any)
                                     - Tool calls (if any)
                                     """
                )
            );

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "Agent",
            ChatHistoryProvider = chatHistoryProvider,
            AIContextProviders = [new CompactionProvider(compactionStrategy, loggerFactory: loggerFactory)],
            ChatOptions = new ChatOptions
            {
                Instructions = "You are a useful assistant. Keep answers very brief.",
            }
        };

        var agent = new ChatClientAgent(chatClient, agentOptions, loggerFactory);

        var content1 = await File.ReadAllTextAsync("data/hamlet-act-1-scene-1.txt");
        var content2 = await File.ReadAllTextAsync("data/hamlet-act-1-scene-2.txt");

        var session = await agent.CreateSessionAsync();

        List<AIContent> backgroundContents = [
            new TextContent($"Here is the background material:\n\n## Act 1, Scene 1\n{content1}\n\n## Act 1, Scene 2\n{content2}")
        ];

        List<AIContent> userContents = [
            new TextContent(userInput),
        ];

        List<ChatMessage> messages = [
            new (ChatRole.User, backgroundContents),
            new (ChatRole.User, userContents),
        ];

        var response1 = await agent.RunAsync(messages, session);

        Console.WriteLine($"  {response1.Text}");

        response1.LogResponseUsage();

        const string statement2 = "Who are the characters in the texts?";

        Console.WriteLine(statement2);

        var response2 = await agent.RunAsync(statement2, session);

        Console.WriteLine($"  {response2.Text}");

        response2.LogResponseUsage();

        const string statement3 = "What are they concerned about? Describe briefly each character's role.";

        Console.WriteLine(statement3);

        var response3 = await agent.RunAsync(statement3, session);

        Console.WriteLine($"  {response3.Text}");

        response3.LogResponseUsage();

        var serializedSession = await agent.SerializeSessionAsync(session);

        Console.WriteLine($"Session, find [Summary] in the following text:\n  {serializedSession.FormatForLogging()}");

        return null;
    }
}
