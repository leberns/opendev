using Azure.Identity;
using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Contracts.Settings;
using Microsoft.Agents.AI;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.AI;

namespace MultiTurnConversation;

public class MeetingHistoryCosmosDb(
    AiSettings aiSettings,
    IChatClientBuilder chatClientBuilder
    ) : IRunnableLab
{
    public List<TagType> GetTags() => [TagType.Conversation, TagType.ChatHistory, TagType.Azure, TagType.CosmosDb];

    public List<string> GetLabDescriptions() => [
        "Demonstrate the agent with chat history stored in Azure Cosmos DB.",
        "This lab requires the Azure Cosmos DB, check in README.md how to provision the infrastructure.",
        "Suggestion: check the collection in Cosmos DB, how the conversation is stored there.",
    ];

    public string GetUserInput() => "In the last meeting John implied the need of expanding. Mary was not so sure.";

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

        // pure previous messages for demo purposes
        await chatHistoryProvider.ClearMessagesAsync(null);

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

        const string statement2 = "Was Mary in favor of expanding?";

        Console.WriteLine(statement2);

        var response2 = await agent.RunAsync(statement2, session);

        Console.WriteLine($"  {response2.Text}");

        var serialized = await agent.SerializeSessionAsync(session);

        Console.WriteLine("Session:");
        Console.WriteLine($"  {serialized}");

        return null;
    }
}
/*
   CosmosDB item contents

   Item for message 1:
{
       "id": "3beabac5-9210-45d5-8d87-da0c70f5ac20",
       "conversationId": "conversation-2026-04-14",
       "timestamp": 1776150817,
       "messageId": null,
       "role": "user",
       "message": "{\"AuthorName\":null,\"CreatedAt\":null,\"Role\":\"user\",\"Contents\":[{\"$type\":\"text\",\"Text\":\"In the last meeting John implied the need of expanding. Mary was not so sure.\",\"Annotations\":null,\"AdditionalProperties\":null}],\"MessageId\":null,\"AdditionalProperties\":null}",
       "type": "ChatMessage",
       "ttl": 86400,
       "tenantId": "tenant-1",
       "userId": "user-1",
       "sessionId": "conversation-2026-04-14",
       "_rid": "QQMRAMHnH8cVAAAAAAAAAA==",
       "_self": "dbs/QQMRAA==/colls/QQMRAMHnH8c=/docs/QQMRAMHnH8cVAAAAAAAAAA==/",
       "_etag": "\"5d01ae75-0000-0d00-0000-69dde9210000\"",
       "_attachments": "attachments/",
       "_ts": 1776150817
   }

   Item for message 2:
{
       "id": "717669d6-ebf7-4439-bb3c-b24a59951f64",
       "conversationId": "conversation-2026-04-14",
       "timestamp": 1776150817,
       "messageId": null,
       "role": "assistant",
       "message": "{\"AuthorName\":\"MeetingAgent\",\"CreatedAt\":\"2026-04-14T07:13:37.532026+00:00\",\"Role\":\"assistant\",\"Contents\":[{\"$type\":\"text\",\"Text\":\"An area of discussion that may lead to further debate.\",\"Annotations\":null,\"AdditionalProperties\":null}],\"MessageId\":null,\"AdditionalProperties\":null}",
       "type": "ChatMessage",
       "ttl": 86400,
       "tenantId": "tenant-1",
       "userId": "user-1",
       "sessionId": "conversation-2026-04-14",
       "_rid": "QQMRAMHnH8cWAAAAAAAAAA==",
       "_self": "dbs/QQMRAA==/colls/QQMRAMHnH8c=/docs/QQMRAMHnH8cWAAAAAAAAAA==/",
       "_etag": "\"5d01af75-0000-0d00-0000-69dde9210000\"",
       "_attachments": "attachments/",
       "_ts": 1776150817
   }

   Item for message 3:
{
       "id": "dc5907b6-990e-4ae9-b725-e10d2d6e19bf",
       "conversationId": "conversation-2026-04-14",
       "timestamp": 1776150818,
       "messageId": null,
       "role": "user",
       "message": "{\"AuthorName\":null,\"CreatedAt\":null,\"Role\":\"user\",\"Contents\":[{\"$type\":\"text\",\"Text\":\"Was Mary in favor of expanding?\",\"Annotations\":null,\"AdditionalProperties\":null}],\"MessageId\":null,\"AdditionalProperties\":null}",
       "type": "ChatMessage",
       "ttl": 86400,
       "tenantId": "tenant-1",
       "userId": "user-1",
       "sessionId": "conversation-2026-04-14",
       "_rid": "QQMRAMHnH8cXAAAAAAAAAA==",
       "_self": "dbs/QQMRAA==/colls/QQMRAMHnH8c=/docs/QQMRAMHnH8cXAAAAAAAAAA==/",
       "_etag": "\"5d015476-0000-0d00-0000-69dde9220000\"",
       "_attachments": "attachments/",
       "_ts": 1776150818
   }

   Item for message 4:
{
       "id": "97534976-d773-4f6f-b55a-2607947193aa",
       "conversationId": "conversation-2026-04-14",
       "timestamp": 1776150818,
       "messageId": null,
       "role": "assistant",
       "message": "{\"AuthorName\":\"MeetingAgent\",\"CreatedAt\":\"2026-04-14T07:13:38.351938+00:00\",\"Role\":\"assistant\",\"Contents\":[{\"$type\":\"text\",\"Text\":\"No, it seems she had reservations.\",\"Annotations\":null,\"AdditionalProperties\":null}],\"MessageId\":null,\"AdditionalProperties\":null}",
       "type": "ChatMessage",
       "ttl": 86400,
       "tenantId": "tenant-1",
       "userId": "user-1",
       "sessionId": "conversation-2026-04-14",
       "_rid": "QQMRAMHnH8cYAAAAAAAAAA==",
       "_self": "dbs/QQMRAA==/colls/QQMRAMHnH8c=/docs/QQMRAMHnH8cYAAAAAAAAAA==/",
       "_etag": "\"5d015576-0000-0d00-0000-69dde9220000\"",
       "_attachments": "attachments/",
       "_ts": 1776150818
   }
 */
