using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MultiTurnConversation;

public class UserPreferencesAiContext(
    IChatClientBuilder chatClientBuilder
    ) : IRunnableLab
{
    public List<TagType> GetTags() => [TagType.Conversation, TagType.ChatHistory, TagType.AiContextProvider, TagType.UserInteraction];

    public List<string> GetLabDescriptions() => [
        "Demonstrate the agent with chat history and a custom AI context provider which stores user preferences.",
        "The user is asked for their location and preferred language.",
        "The context provider injects these preferences into every agent invocation.",
        "The InMemory session retains chat history across turns within the same run.",
    ];

    public string GetUserInput() => "What time zone am I in, and what language should you use?";

    public async Task<string?> RunLabAsync(string userInput)
    {
        Console.Write("Enter the user location (e.g. Zurich, New York): ");
        var location = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter the user preferred language (e.g. English, German, French): ");
        var language = Console.ReadLine() ?? string.Empty;

        var preferences = new UserPreferences(location, language);

        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaLlama);

        var userPreferencesMemory = new UserPreferencesMemory(preferences);

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "AssistantAgent",
            AIContextProviders = [userPreferencesMemory],
            ChatOptions = new ChatOptions
            {
                Instructions = "You are a helpful assistant. Keep answers very brief.",
                Temperature = 0.1f,
            }
        };

        var agent = new ChatClientAgent(chatClient, agentOptions);

        var session = await agent.CreateSessionAsync();

        var response1 = await agent.RunAsync(userInput, session);

        Console.WriteLine($"{response1.Text}");

        const string question2 = "What is a typical greeting in my language?";

        Console.WriteLine();
        Console.WriteLine(question2);

        var response2 = await agent.RunAsync(question2, session);

        Console.WriteLine($"{response2.Text}");

        return null;
    }

    internal class UserPreferencesMemory(UserPreferences preferences) : AIContextProvider
    {
        protected override async ValueTask<AIContext> ProvideAIContextAsync(
            InvokingContext context,
            CancellationToken cancellationToken = default)
        {
            var aiContext = new AIContext
            {
                Instructions = $"The user is located in {preferences.Location}. Their preferred language is {preferences.Language}. " +
                               $"Adapt your responses to their location and use their preferred language when appropriate."
            };

            return await Task.FromResult(aiContext);
        }
    }

    internal record UserPreferences(string Location, string Language);
}
