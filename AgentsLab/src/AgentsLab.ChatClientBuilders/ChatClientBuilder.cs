using Contracts.ChatClientBuilders;
using Microsoft.Extensions.AI;

namespace ChatClientBuilders;

public class ChatClientBuilder(
    IEnumerable<ILabChatFactory> chatClientFactories
    ) : IChatClientBuilder
{
    public IChatClient BuildChatClient(ChatClientType chatClientType)
    {
        var chatClient = chatClientFactories
            .FirstOrDefault(factory => factory.CanCreate(chatClientType))?
            .CreateChatClient(chatClientType)
            ?? throw new ArgumentOutOfRangeException(
                nameof(chatClientType),
                chatClientType,
                $"No chat client factory found for type {chatClientType}, look at the factories, add or update them, as needed");

        return chatClient;
    }
}