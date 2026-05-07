using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentExtensions;

public static class LogExtensions
{
    public static void LogResponseAndUsage(this AgentResponse response)
    {
        var usage = response.Usage;

        Console.WriteLine($"Response text: {response.Text}");
        Console.WriteLine($"Finish reason: {response.FinishReason}");
        LogUsage(usage);
    }

    public static void LogResponseUsage(this AgentResponse response)
    {
        response.Usage.LogUsage();
    }

    public static void LogUsage(this UsageDetails? usage)
    {
        Console.WriteLine($"Tokens usage - input: {usage?.InputTokenCount}");
        Console.WriteLine($"Tokens usage - output: {usage?.OutputTokenCount}");
        Console.WriteLine($"Tokens usage - reasoning: {usage?.ReasoningTokenCount}");
        Console.WriteLine($"Tokens usage - total: {usage?.TotalTokenCount}");
    }

    public static string FormatForLogging(this JsonElement serializedSession)
    {
        var sessionStr = serializedSession.ToString();

        const int maxLength = 1450;

        var cutLength = sessionStr.Length > maxLength ? maxLength : sessionStr.Length;

        var formatedStr = sessionStr[..cutLength];

        if (cutLength == maxLength)
        {
            formatedStr += $"... [truncated, full length: {sessionStr.Length} characters]";
        }

        return formatedStr;
    }
}