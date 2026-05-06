using System.Text;
using AgentExtensions;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;

namespace TokensStreamer;

public class ReasoningThinkingAgent(
    IChatClientBuilder chatClientBuilder
) : IRunnableLab
{
    public List<LabTagType> GetTags() => [LabTagType.Streaming, LabTagType.Tokens, LabTagType.Thinking];

    public List<string> GetLabDescriptions() => [
        "Demonstrate reasoning to resolve a math problem.",
        "The thinking steps can be seen as they are streamed to the console.",
    ];

    public string GetUserInput() =>
        "Paul bought 3 oranges and 2 apples. Mary bought 1 orange and ate one of the Paul's apples. How many oranges and apples are remaining?";

    public async Task<string?> RunLabAsync(string userInput)
    {
        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaQwenThinking4B);

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "Agent",
            ChatOptions = new ChatOptions
            {
                Reasoning = new ReasoningOptions
                {
                    Effort = ReasoningEffort.Low,
                    Output = ReasoningOutput.Summary
                },
                Instructions = "You are a useful assistant."
            }
        };

        var agent = new ChatClientAgent(chatClient, agentOptions);

        StringBuilder sb = new();

        await foreach (var message in agent.RunStreamingAsync(userInput))
        {
            foreach (var content in message.Contents)
            {
                switch (content)
                {
                    case TextReasoningContent thinking:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(thinking.Text);
                        Console.ResetColor();
                        break;

                    case TextContent textContent:
                        sb.Append(textContent.Text);
                        break;

                    case UsageContent usage:
                        usage.Details.LogUsage();
                        break;
                }
            }
        }

        return sb.ToString();
    }
}