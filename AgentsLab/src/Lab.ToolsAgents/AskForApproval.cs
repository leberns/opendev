﻿using System.ComponentModel;
using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Lab.MiddlewareAgents;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Lab.ToolsAgents;

public class AskForApproval(
    IChatClientBuilder chatClientBuilder
    ) : IRunnableLab
{
    public List<TagType> GetTags() => [
        TagType.Tools,
        TagType.UserInteraction,
        TagType.HumanInTheLoop];

    public List<string> GetLabDescriptions() => [
        "A tool is wrapped by another tool that asks for confirmation from the user before the agent can take an action."];

    public string GetUserInput() => "Create a support ticket to provision a VM.";

    public async Task<string?> RunLabAsync(string userInput)
    {
        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaQwenVl4B);

        // The tool is wrapped with approval logic, so the user must confirm before it executes
        var submitTicketWithApproval = AIFunctionFactory.Create(
            ([Description("Description.")] string description) =>
            {
                Console.WriteLine();
                Console.WriteLine($"The agent wants to call: {nameof(AskForApprovalTool.SubmitSupportTicket)}, description: {description}");
                Console.Write("Approve? (y/n): ");
                var answer = Console.ReadLine();

                if (string.Equals(answer?.Trim(), "y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Tool call approved by the user");
                    return AskForApprovalTool.SubmitSupportTicket(description);
                }

                Console.WriteLine("Tool call rejected by the user.");
                return "The user rejected the tool call.";
            },
            nameof(AskForApprovalTool.SubmitSupportTicket),
            "Submit a support ticket.");

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "Agent",
            ChatOptions = new ChatOptions
            {
                Instructions = "You are a support agent. You might create support tickets if the user asks this. " +
                               "Use the tool SubmitSupportTicket to create a support ticket." +
                               "Do not ask any extra information to the user, just create the ticket." +
                               "If the user rejects the tool call, just say that and stop trying to execute the tool.",
                Tools = [submitTicketWithApproval],
                Temperature = 0.1f,
            }
        };

        var aiAgent = new ChatClientAgent(chatClient, agentOptions);

        var chatMessage = new ChatMessage(ChatRole.User, userInput);

        var response = await aiAgent.RunAsync(chatMessage);

        return response.Text;
    }
}
