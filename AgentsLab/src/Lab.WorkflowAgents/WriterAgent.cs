using AgentExtensions;
using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace WriterAgency;

public class WriterAgent(
    IChatClientBuilder chatClientBuilder
) : IRunnableLab
{
    public List<TagType> GetTags() => [TagType.Tools];

    public List<string> GetLabDescriptions() => [
        "Demonstrate an agent that creates an story.",
        "It can use tools to format the output.",
    ];

    public string GetUserInput() => "Topic: ghost town.";

    public async Task<string?> RunLabAsync(string userInput)
    {
        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaLlama);

        AIAgent agent = new ChatClientAgent(
            chatClient,
            instructions: "Write the beginning of stories with up to 50 words that are engaging and creative. " +
                          "Review them and make sure they are suitable for children.",
            name: "Writer",
            tools:
            [
                AIFunctionFactory.Create(WritingAgencyTools.GetAuthorTool),
                AIFunctionFactory.Create(WritingAgencyTools.FormatStoryTool)
            ]);

        var response = await agent.RunAsync(userInput);

        response.LogResponseUsage();

        return response.Text;
    }
}
