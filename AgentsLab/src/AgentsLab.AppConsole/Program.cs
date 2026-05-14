using AgentsConsole;
using ChatClientBuilders;
using ChatClientBuilders.Factories;
using Contracts;
using Contracts.ChatClientBuilders;
using Microsoft.Extensions.Configuration;
using Contracts.Settings;
using DataUnderstanding;
using Lab.MiddlewareAgents;
using Lab.ToolsAgents;
using McpAgents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MultiTurnConversation;
using SentimentAnalyser;
using TokensStreamer;
using WriterAgency;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var aiSettings = configuration.GetSection(nameof(AiSettings)).Get<AiSettings>();

if (aiSettings is null)
{
    throw new InvalidOperationException("AI settings not found in configuration");
}

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddConfiguration(configuration.GetSection("Logging"));
});

services.AddSingleton(aiSettings);
services.AddSingleton<AgentsRunner>();
services.AddTransient<IChatClientBuilder, ChatClientBuilder>();
services.AddTransient<ILabChatFactory, OllamaChatFactory>();
services.AddTransient<ILabChatFactory, AzureOpenAiChatFactory>();
services.AddTransient<ILabChatFactory, GitHubCopilotChatClientFactory>();
services.AddTransient<IRunnableLab, TokensStreamerAgent>();
services.AddTransient<IRunnableLab, ReasoningThinkingAgent>();
services.AddTransient<IRunnableLab, SentimentAnalyserChat>();
services.AddTransient<IRunnableLab, SentimentAnalyserAgent>();
services.AddTransient<IRunnableLab, ImageDataExtractionAgent>();
services.AddTransient<IRunnableLab, PdfImageTextExtractionAgent>();
services.AddTransient<IRunnableLab, MeetingNoSession>();
services.AddTransient<IRunnableLab, MeetingHistoryInMemory>();
services.AddTransient<IRunnableLab, MeetingHistoryCosmosDb>();
services.AddTransient<IRunnableLab, MeetingHistoryCosmosDbCont>();
services.AddTransient<IRunnableLab, TravelAdvisorAgent>();
services.AddTransient<IRunnableLab, UserPreferencesAiContext>();
services.AddTransient<IRunnableLab, ConversationCompaction>();
services.AddTransient<IRunnableLab, GuardrailMiddlewareAgent>();
services.AddTransient<IRunnableLab, WriterAgent>();
services.AddTransient<IRunnableLab, WorkflowWritingAgency>();
services.AddTransient<IRunnableLab, AskForApproval>();
services.AddTransient<IRunnableLab, McpSubmitExpenseAgent>();
services.AddTransient<IRunnableLab, McpServicesAgent>();
services.AddTransient<IRunnableLab, McpServicesAgency>();

var serviceProvider = services.BuildServiceProvider();

var runner = serviceProvider.GetRequiredService<AgentsRunner>();
await runner.RunAsync();