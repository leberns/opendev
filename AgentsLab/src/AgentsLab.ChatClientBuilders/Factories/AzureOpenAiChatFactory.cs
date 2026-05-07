using Azure.AI.OpenAI;
using Azure.Identity;
using Contracts.ChatClientBuilders;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace ChatClientBuilders.Factories;

public class AzureOpenAiChatFactory(
    ILogger<AzureOpenAiChatFactory> logger
    ) : ILabChatFactory
{
    public bool CanCreate(ChatClientType chatClientType) => chatClientType == ChatClientType.AzureOpenAi;

    public IChatClient CreateChatClientAsync(ChatClientType chatClientType)
    {
        /*
         * - Deploy to Azure Foundry to have OpenAI and a deployment and update the environment variables. Ex.:
           ```Sh
           export AZURE_OPENAI_ENDPOINT="https://cog-mo7xupkdn22ec.openai.azure.com/"
           export AZURE_OPENAI_DEPLOYMENT_NAME="chat-model"
           ```
         */

        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
                       ?? throw new InvalidOperationException("Set AZURE_OPENAI_ENDPOINT");

        var deploymentName = chatClientType.ChatClientTypeToModel();

        var aiClient = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential());

        logger.LogInformation("{DateTime} | Creating chat client, type {ChatClientType}, chat client {Name}, model / deployment {Model}",
            DateTime.Now,
            chatClientType,
            aiClient.GetType().Name,
            deploymentName);

        return aiClient.GetChatClient(deploymentName).AsIChatClient();
    }
}
