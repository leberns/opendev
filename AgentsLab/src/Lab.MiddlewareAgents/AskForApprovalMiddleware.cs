using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Lab.MiddlewareAgents;

public class AskForApprovalMiddleware(
    IChatClientBuilder chatClientBuilder
    ) : IRunnableLab
{
    public List<TagType> GetTags() => [
        TagType.Tools,
        TagType.Middleware,
        TagType.UserInteraction,
        TagType.HumanInTheLoop];

    public List<string> GetLabDescriptions() => [
        "A middleware asks for confirmation from the user before the agent can take an action, like call a tool."];

    public string GetUserInput() => "Create a support ticket to provision a VM.";

    public async Task<string?> RunLabAsync(string userInput)
    {
        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaQwenVl4B);

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "Agent",
            ChatOptions = new ChatOptions
            {
                Instructions = "You are a support agent. You might create support tickets if the user asks this. " +
                               "Use the tool SubmitSupportTicket to create a support ticket." +
                                "Do not ask any extra information to the user, just create the ticket.",
                Tools = [AIFunctionFactory.Create(AskForApprovalTool.SubmitSupportTicket)],
                Temperature = 0.1f,
            }
        };

        var aiAgent = new ChatClientAgent(chatClient, agentOptions);

        var middlewareEnabledAgent = aiAgent
            .AsBuilder()
            .Use(ConfirmationMiddleware, null)
            .Build();

        var chatMessage = new ChatMessage(ChatRole.User, userInput);

        var response = await middlewareEnabledAgent.RunAsync(chatMessage);

        return response.Text;
    }

    private async Task<AgentResponse> ConfirmationMiddleware(
        IEnumerable<ChatMessage> messages,
        AgentSession? session,
        AgentRunOptions? options,
        AIAgent innerAgent,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"{nameof(ConfirmationMiddleware)}: running middleware for {innerAgent.Name}");
        Console.WriteLine();

        var innerResponse = await innerAgent.RunAsync(messages, session, options, cancellationToken);

        Console.WriteLine($"{nameof(ConfirmationMiddleware)}:");
        Console.WriteLine($"{innerResponse.Text}");
        Console.WriteLine();

        return innerResponse;
    }
}
