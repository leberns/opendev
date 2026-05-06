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
    public List<LabTagType> GetTags() => [LabTagType.MultiModal, LabTagType.Vision, LabTagType.DataExtraction];

    public List<string> GetLabDescriptions() => [
        "Demonstrate an agent extracting visual information from a form image.",
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

        var response = await agent.RunAsync<ExtractedContent>(messages);

        response.LogResponseAndUsage();

        Console.WriteLine($"Extracted card, name and address: {response.Result.CardChoice}, {response.Result.Name}, {response.Result.Address}");

        return null;
    }
}

public record ExtractedContent(string CardChoice, string Name, string Address);
