using Contracts.ChatClientBuilders;

namespace ChatClientBuilders;

public static class ChatClientTypeToModelsMapping
{
    /// <summary>
    /// Which (LLM) model to use for the given chat client type.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// bug: received an unexpected type to map, update the mapping below</exception>
    public static string ChatClientTypeToModel(this ChatClientType chatClientType)
    {
        return chatClientType switch
        {
            ChatClientType.OllamaLlama => "llama3.2",
            ChatClientType.OllamaLlamaVision => "llama3.2-vision",
            ChatClientType.OllamaQwenThinking4B => "qwen3.5:4b",
            ChatClientType.OllamaQwenVl4B => "qwen3-vl:4b",
            ChatClientType.OllamaQwenVl8B => "qwen3-vl:8b",
            ChatClientType.AzureOpenAi => "gpt-4o-mini",
            ChatClientType.GitHubCopilot => "gpt-4o-mini",
            _ => throw new ArgumentOutOfRangeException(nameof(chatClientType), chatClientType, null)
        };
    }
}