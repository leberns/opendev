using Microsoft.Extensions.AI;

namespace Contracts.ChatClientBuilders;

public interface ILabChatFactory
{
    /// <summary>
    /// Is this the factory to create concrete chat clients for the given type?
    /// </summary>
    /// <param name="chatClientType"></param>
    /// <returns></returns>
    bool CanCreate(ChatClientType chatClientType);

    /// <summary>
    /// Instantiate a concrete chat client
    /// </summary>
    IChatClient CreateChatClient(ChatClientType chatClientType);
}