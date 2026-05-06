using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MultiTurnConversation;

public class TravelAdvisorAgent(
    IChatClientBuilder chatClientBuilder
) : IRunnableLab
{
    public List<LabTagType> GetTags() => [LabTagType.Conversation, LabTagType.ChatHistory, LabTagType.Tools];

    public List<string> GetLabDescriptions() => [
        "Demonstrate a travel advisor agent with chat history stored in memory.",
        "Because there is history the agent can answer questions based on previous messages.",
        "For example, the agent knows the pronoun 'there' refers to Paris in the messages.",
        "The agent can also call a tool telling the weather forecast.",
    ];

    public string GetUserInput() => "What is the largest city in France?";

    public async Task<string?> RunLabAsync(string userInput)
    {
        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaQwenVl4B);

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "TravelAdvisorAgent",
            ChatOptions = new ChatOptions
            {
                Instructions = "You are a friendly travel assistant. Keep answers very brief.",
                Tools = [AIFunctionFactory.Create(TravelAdvisorWeatherTool.GetWeather)]
            }
        };

        var agent = new ChatClientAgent(chatClient, agentOptions);

        var session = await agent.CreateSessionAsync();

        Console.WriteLine(userInput);

        var response1 = await agent.RunAsync(userInput, session);

        Console.WriteLine($"  {response1.Text}");

        const string question2 = "What are the top 3 things to visit there?";

        Console.WriteLine(question2);

        var response2 = await agent.RunAsync(question2, session);

        Console.WriteLine($"  {response2.Text}");

        const string question3 = "What is the weather like there?";

        Console.WriteLine(question3);

        var response3 = await agent.RunAsync(question3, session);

        Console.WriteLine($"  {response3.Text}");

        return null;
    }
}
