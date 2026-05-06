using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MultiTurnConversation;

public class MeetingNoSession(
    IChatClientBuilder chatClientBuilder
    ) : IRunnableLab
{
    public List<LabTagType> GetTags() => [];

    public List<string> GetLabDescriptions() => [
        "Demonstrate the agent without chat history.",
        "The history enables agents to perform multi-turn conversations.",
        "Because there is no history the agent cannot answer questions based on previous user inputs, like seen in this lab.",
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

        var response1 = await agent.RunAsync(userInput);

        Console.WriteLine($"  {response1.Text}");

        const string statement2 = "Was Mary in favor of expanding?";

        Console.WriteLine(statement2);

        var response2S = await agent.RunAsync(statement2);

        Console.WriteLine($"  {response2S.Text}");

        return null;
    }
}
