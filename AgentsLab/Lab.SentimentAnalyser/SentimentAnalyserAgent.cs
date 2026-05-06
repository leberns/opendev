using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace SentimentAnalyser;

public class SentimentAnalyserAgent(
    IChatClientBuilder chatClientBuilder
    ) : IRunnableLab
{
    public List<LabTagType> GetTags() => [LabTagType.SentimentAnalyser, LabTagType.DataExtraction];

    public List<string> GetLabDescriptions() => [
        "Demonstrate the analysis and extraction of sentiment data from text using an agent.",
        "The temperature is set to a lower value so that the most high scored next tokens are considered, that means less varied answer.",
    ];

    public string GetUserInput() => "Today is a happy day!";

    public async Task<string?> RunLabAsync(string userInput)
    {
        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaLlama);

        ChatClientAgentOptions agentOptions = new()
        {
            Name = "Agent",
            ChatOptions = new ChatOptions
            {
                Instructions = "You are an sentiment analyser. " +
                              "Analyze the sentiment of the following text and respond with ONLY one word: Positive, Negative, or Neutral. " +
                              "Provide also a 'confidence' value between 0 and 1 about how accurate is this analysis. " +
                              "The confidence has to be comma separated from the sentiment.",
                Temperature = 0.1f,
            }
        };

        var aiAgent = new ChatClientAgent(chatClient, agentOptions);

        var chatMessage = new ChatMessage(ChatRole.User, userInput);

        var response = await aiAgent.RunAsync<SentimentData>(chatMessage);

        var result = response.Result.Sentiment + ", " + response.Result.Confidence;

        return result;
    }
}

public record SentimentData(string Sentiment, float Confidence);