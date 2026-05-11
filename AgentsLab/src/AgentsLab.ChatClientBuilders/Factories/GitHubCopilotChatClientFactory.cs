using System.ClientModel;
using Contracts.ChatClientBuilders;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;

namespace ChatClientBuilders.Factories;

public class GitHubCopilotChatClientFactory(
    ILogger<GitHubCopilotChatClientFactory> logger
    ) : ILabChatFactory
{
    public bool CanCreate(ChatClientType chatClientType) => chatClientType == ChatClientType.GitHubCopilot;

    public IChatClient CreateChatClient(ChatClientType chatClientType)
    {
        if (Environment.GetEnvironmentVariable("GITHUB_TOKEN") is null)
        {
            throw new InvalidOperationException("GITHUB_TOKEN environment variable is not set");
        }

        var model = chatClientType.ChatClientTypeToModel();

        var chatClient = new ChatClient(
                model,
                new ApiKeyCredential(Environment.GetEnvironmentVariable("GITHUB_TOKEN")!),
                new OpenAIClientOptions { Endpoint = new Uri("https://models.github.ai/inference") })
            .AsIChatClient();

        logger.LogInformation("{DateTime} | Creating chat client, type {ChatClientType}, chat client {Name}, model {Model}",
            DateTime.Now,
            chatClientType,
            chatClient.GetType().Name,
            model);

        return chatClient;
    }
}