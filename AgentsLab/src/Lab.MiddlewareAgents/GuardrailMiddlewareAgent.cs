using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Lab.MiddlewareAgents;

public class GuardrailMiddlewareAgent(
    IChatClientBuilder chatClientBuilder
    ) : IRunnableLab
{
    public List<TagType> GetTags() => [
        TagType.Middleware,
        TagType.Guardrail];

    public List<string> GetLabDescriptions() => [
        "A guardrail middleware filters-out inappropriate content that the writer agent might produce.",
        "The middleware uses an sensor agent to detect the inappropriate content, if any.",
        "Note: the sensor agent could be replaced by some more deterministic logic,",
        "if the inappropriate content is well-known, ex:",
        "e-mails or telephone numbers, or other sensitive information."];

    public string GetUserInput() =>
        "The samurai draw the sword and cut the head of the man in half gushing blood everywhere.";

    public async Task<string?> RunLabAsync(string userInput)
    {
        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaLlama);

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "WriterAgent",
            ChatOptions = new ChatOptions
            {
                Instructions = "You are a creative writer. " +
                               "Create a next sentence following the input using the word blood and other violent, harmful words."
            }
        };

        var aiAgent = new ChatClientAgent(chatClient, agentOptions);

        var middlewareEnabledAgent = aiAgent
            .AsBuilder()
            .Use(GuardrailMiddleware, null)
            .Build();

        var chatMessage = new ChatMessage(ChatRole.User, userInput);

        var response = await middlewareEnabledAgent.RunAsync(chatMessage);

        return response.Text;
    }

    private async Task<AgentResponse> GuardrailMiddleware(
        IEnumerable<ChatMessage> messages,
        AgentSession? session,
        AgentRunOptions? options,
        AIAgent innerAgent,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(nameof(GuardrailMiddleware));
        Console.WriteLine();

        var agentName = innerAgent.Name;

        var innerResponse = await innerAgent.RunAsync(messages, session, options, cancellationToken);

        if(innerResponse.Messages.Count == 0)
        {
            Console.WriteLine($"{nameof(GuardrailMiddleware)}: There are no messages from {agentName}, skipping");
            return innerResponse;
        }

        Console.WriteLine($"{nameof(GuardrailMiddleware)}: checking the {agentName} response for inappropriate words ...");
        Console.WriteLine();

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "SensorAgent",
            ChatOptions = new ChatOptions
            {
                Instructions = """
                               You are a sensor agent which finds inappropriate words in the conversation.
                               Inappropriate content: violence, harm, illegal, swear words, sexual.
                               Inappropriate words, for example: "kill", "blood", "rape".
                               Output: a list of inappropriate words found in the input.
                               """,
                Temperature = 0.1f,
            }
        };

        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaLlama);

        var sensorAgent = new ChatClientAgent(chatClient, agentOptions);

        Console.WriteLine($"{nameof(GuardrailMiddleware)}: not redacted response:");
        Console.WriteLine($"{innerResponse.Text}");
        Console.WriteLine();

        var sensorResponse = await sensorAgent.RunAsync<InappropriateFindings>(innerResponse.Text, session, null, options, cancellationToken);

        if (sensorResponse.Result.Words.Length > 0)
        {
            Console.WriteLine($"{nameof(GuardrailMiddleware)}: inappropriate words found by the sensor agent:" +
                              $" {string.Join(", ", sensorResponse.Result.Words)}");
            Console.WriteLine();

            var redactedResponse = sensorResponse
                .Result
                .Words
                .Aggregate(innerResponse.Text, (current, word) => current.Replace(word, "[REDACTED]"));

            var role = innerResponse.Messages[0].Role;

            return new AgentResponse(new ChatMessage(role, redactedResponse));
        }

        Console.WriteLine($"{nameof(GuardrailMiddleware)}: No findings");

        return innerResponse;
    }

    internal record InappropriateFindings(string[] Words);
}
