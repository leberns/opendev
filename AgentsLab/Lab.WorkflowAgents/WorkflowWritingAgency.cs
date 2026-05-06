using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace WriterAgency;

public class WorkflowWritingAgency(
    IChatClientBuilder chatClientBuilder,
    ILoggerFactory loggerFactory
    ) : IRunnableLab
{
    public List<LabTagType> GetTags() => [LabTagType.Tools, LabTagType.MultiAgent, LabTagType.Workflow];

    public List<string> GetLabDescriptions() => [
        "Demonstrate a sequential agent workflow that creates an story.",
        "The writer agent can use tools to format the output.",
        "The editor agent can review the story and make changes.",
    ];

    public string GetUserInput() => "Topic: ghost town.";

    public async Task<string?> RunLabAsync(string userInput)
    {
        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaLlama);

        AIAgent writer = new ChatClientAgent(
            chatClient,
            instructions: "Write the beginning of stories with up to 50 words that are engaging and creative.",
            name: "Writer",
            tools:
            [
                AIFunctionFactory.Create(WritingAgencyTools.GetAuthorTool),
                AIFunctionFactory.Create(WritingAgencyTools.FormatStoryTool)
            ],
            loggerFactory: loggerFactory);

        AIAgent editor = new ChatClientAgent(
            chatClient,
            instructions:
            "Make the beginning of the story suitable for children keeping it still up to 50 words, fix grammar, and enhance the plot.",
            name: "Editor",
            loggerFactory: loggerFactory);

        var workflow = AgentWorkflowBuilder.BuildSequential(writer, editor);

        var workflowAgent = workflow.AsAIAgent();

        var workflowResponse = await workflowAgent.RunAsync(userInput);

        return workflowResponse.Text;
    }
}
