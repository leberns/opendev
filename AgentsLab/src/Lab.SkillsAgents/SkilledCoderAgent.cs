using System.Diagnostics.CodeAnalysis;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;

namespace Lab.SkillsAgents;

public class SkilledCoderAgent(
    IChatClientBuilder chatClientBuilder
    ) : IRunnableLab
{
    public List<TagType> GetTags() => [
        TagType.AiContextProvider,
        TagType.Skills
    ];

    public List<string> GetLabDescriptions() => [
        "Demonstrate skills added to an agent.",
        "For instance, specific coding skills can be found here: https://microsoft.github.io/skills/",
    ];

    public string GetUserInput() =>
        "Write a login method  in C#.";

    [Experimental("MAAI001")]
    public async Task<string?> RunLabAsync(string userInput)
    {
        var skillsProvider = new AgentSkillsProvider("Skills");

        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaQwenCoder30B);

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "CoderAgent",
            ChatOptions = new ChatOptions
            {
                Instructions = "You are a coder agent."
            },
            AIContextProviders = [skillsProvider]
        };

        var aiAgent = new ChatClientAgent(chatClient, agentOptions);

        var chatMessage = new ChatMessage(ChatRole.User, userInput);

        var response = await aiAgent.RunAsync(chatMessage);

        return response.Text;
    }
}
