using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

using AgentExtensions;
using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;

namespace TokensStreamer;

public class TokensStreamerAgent(
    IChatClientBuilder chatClientBuilder
    ) : IRunnableLab
{
    public List<TagType> GetTags() => [TagType.Streaming, TagType.Tokens];

    public List<string> GetLabDescriptions() => [
        "Demonstrate response tokens, streaming and tokens usage.",
        "The instruction is to output the answer in capital case, just to demonstrate how it works.",
    ];

    public string GetUserInput() => "What are the rainbow colors?";

    public async Task<string?> RunLabAsync(string userInput)
    {
        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaLlama);

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "Agent",
            ChatOptions = new ChatOptions
            {
                Instructions = "You are a useful assistant. Keep answers very brief. Output all words in capital case."
            }
        };

        var agent = new ChatClientAgent(chatClient, agentOptions);

        var response = await agent.RunAsync(userInput);

        Console.WriteLine("Without streaming:");

        response.LogResponseAndUsage();

        Console.WriteLine();
        Console.WriteLine("With streaming:");

        Console.Write("The output shows the stream with ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Token ");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("(ContinuationToken, FinishReason, Role) ...");
        Console.ResetColor();

        int length = 0;
        await foreach (var update in agent.RunStreamingAsync(userInput))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{update.Text}");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" ({update.ContinuationToken}, {update.FinishReason}, {update.Role}) ");
            Console.ResetColor();

            length ++;
        }

        Console.WriteLine();
        Console.WriteLine($"Total tokens streamed: {length}");

        return null;
    }
}
