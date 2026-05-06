using Microsoft.Extensions.AI;

namespace Contracts.ChatClientBuilders;

public interface IChatClientBuilder
{
    /// <summary>
    /// Create an instance of a chat with the specified LLM / model.
    /// </summary>
    /// <param name="chatClientType">the chat client type defines the chat client (Ollama, GitHub Copilot, etc) and the model to use</param>
    IChatClient BuildChatClient(ChatClientType chatClientType);
}