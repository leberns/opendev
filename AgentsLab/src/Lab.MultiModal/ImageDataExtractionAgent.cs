using System.Diagnostics;
using AgentExtensions;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;

namespace DataUnderstanding;

public class ImageDataExtractionAgent(
    IChatClientBuilder chatClientBuilder
    ) : IRunnableLab
{
    public List<TagType> GetTags() => [TagType.MultiModal, TagType.Vision, TagType.DataExtraction];

    public List<string> GetLabDescriptions() => [
        "Demonstrate an agent extracting visual information from an image.",
        "The image is a form requesting an additional credit card, fields: card choice, name and address.",
        "Expected output:",
        " {\"cardChoice\": \"World Mastercard® Pink\", \"name\": \"John Blue\", \"address\": \"Bahnhofstrasse 23, 8001, Zürich, Schweiz\"}",
        "",
        "Please note it might take one or two minutes to execute, depending on hardware.",
    ];

    public string GetUserInput() => "Extract the card choice, name and address from the document.";

    public async Task<string?> RunLabAsync(string userInput)
    {
        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaQwenVl4B);

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "Agent",
            ChatOptions = new ChatOptions
            {
                Instructions = "You are a helpful assistant. Keep answers very brief."
            }
        };

        var agent = new ChatClientAgent(chatClient, agentOptions);

        var contentBytes = await File.ReadAllBytesAsync("data/additional-card.png");

        Console.WriteLine($"Data file content bytes: {contentBytes.Length}");

        var rom = new ReadOnlyMemory<byte>(contentBytes);

        List<AIContent> contents = [
            new TextContent(userInput),
            new DataContent(rom, "image/png")
        ];

        List<ChatMessage> messages = [
            new (ChatRole.User, contents)
        ];

        long startTime = Stopwatch.GetTimestamp();

        var response = await agent.RunAsync<ExtractedContent>(messages);

        var elapsedTime = Stopwatch.GetElapsedTime(startTime);

        response.LogResponseAndUsage();

        Console.WriteLine($"The image processing took {elapsedTime.TotalSeconds} s");

        return null;
    }
}

public record ExtractedContent(string CardChoice, string Name, string Address);
