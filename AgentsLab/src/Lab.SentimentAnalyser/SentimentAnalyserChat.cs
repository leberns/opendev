using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;
using Microsoft.Extensions.AI;

namespace SentimentAnalyser;

public class SentimentAnalyserChat(
    IChatClientBuilder chatClientBuilder
    ) : IRunnableLab
{
    public List<TagType> GetTags() => [TagType.SentimentAnalyser, TagType.Chat];

    public List<string> GetLabDescriptions() => [
        "Demonstrate the analysis of the sentiment from a text using a LLM without an agent.",
        "The sentiment data is outputted in the chat answer.",
        "The temperature is set to a lower value so that just the most high scored next tokens are considered, that means less varied answer."
    ];

    public string GetUserInput() => "Today is a happy day!";

    public async Task<string?> RunLabAsync(string userInput)
    {
        var chatClient = chatClientBuilder.BuildChatClient(ChatClientType.OllamaLlama);

        var options = new ChatOptions
        {
            ResponseFormat = ChatResponseFormat.Text,
            Temperature = 0.1f, // scale the logits before applying softmax, like "contrast"; lower value less random outcome
            TopP = 0.1f, // nucleus sampling, with 0.1 just the top 10% probable tokens are considered
            TopK = 1 // number of next tokens to be considered
        };

        var chatResponse = await chatClient.GetResponseAsync(
            [
                new ChatMessage(ChatRole.System,
                    "You are an sentiment analyser. " +
                    "Analyze the sentiment of the following text and respond with ONLY one word: Positive, Negative, or Neutral. " +
                    "Provide also a 'confidence' value between 0 and 1 about how accurate is this analysis. " +
                    "The confidence has to be comma separated from the sentiment."),
                new ChatMessage(ChatRole.User, userInput)
            ],
            options,
            CancellationToken.None);

        return chatResponse.Text;
    }
}