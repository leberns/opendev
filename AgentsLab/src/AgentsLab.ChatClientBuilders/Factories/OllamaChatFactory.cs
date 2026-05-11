using Contracts.ChatClientBuilders;
using Contracts.Settings;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using OllamaSharp;

namespace ChatClientBuilders.Factories;

public class OllamaChatFactory(
    AiSettings aiSettings,
    ILogger<OllamaChatFactory> logger
    ) : ILabChatFactory
{
    public bool CanCreate(ChatClientType chatClientType) =>
        chatClientType is
            ChatClientType.OllamaLlama
            or ChatClientType.OllamaLlamaVision
            or ChatClientType.OllamaQwenThinking4B
            or ChatClientType.OllamaQwenVl4B
            or ChatClientType.OllamaQwenVl8B;

    public IChatClient CreateChatClient(ChatClientType chatClientType)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(aiSettings.OllamaEndpoint),
            Timeout = TimeSpan.FromMinutes(5)
        };

        var model = chatClientType.ChatClientTypeToModel();

        var chatClient = new OllamaApiClient(httpClient, model);

        logger.LogInformation("{DateTime} | Creating chat client, type {ChatClientType}, chat client {Name}, model {Model}",
            DateTime.Now,
            chatClientType,
            chatClient.GetType().Name,
            model);

        return chatClient;
    }
}
