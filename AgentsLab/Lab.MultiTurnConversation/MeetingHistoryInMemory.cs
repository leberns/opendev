using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MultiTurnConversation;

public class MeetingHistoryInMemory(
    IChatClientBuilder chatClientBuilder
    ) : IRunnableLab
{
    public List<LabTagType> GetTags() => [LabTagType.Conversation, LabTagType.ChatHistory];

    public List<string> GetLabDescriptions() => [
        "Demonstrate the agent with chat history stored in memory.",
        "The history enables agents to perform multi-turn conversations.",
        "Without history the agent has no recollection of the previous messages, there is actually no or very limited dialog.",
        "The history is stored in memory, therefore it is not persisted or shared between processes or requests, for instance.",
    ];

    public string GetUserInput() => "In the last meeting John implied the need of expanding. Mary was not so sure.";

    public async Task<string?> RunLabAsync(string userInput)
    {
        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaLlama);

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "MeetingAgent",
            ChatOptions = new ChatOptions
            {
                Instructions = "You are a friendly assistant. " +
                               "Answer the questions when asked, otherwise just observe the conversation. " +
                               "Keep answers very brief.",
                Temperature = 0.1f,
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

        return null;
    }
}
