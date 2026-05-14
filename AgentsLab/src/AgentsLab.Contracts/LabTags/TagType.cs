namespace Contracts.LabTags;

/// <summary>
/// Features and special characteristics of the lab having the tag.
/// </summary>
public enum TagType
{
    /// <summary>
    /// Chat without an agent
    /// </summary>
    Chat = 1,

    /// <summary>
    /// Conversation, dialogue with agent
    /// </summary>
    Conversation = 2,

    /// <summary>
    /// Chat history, conversation context
    /// </summary>
    ChatHistory = 3,

    /// <summary>
    /// Thinking, reasoning
    /// </summary>
    Thinking = 4,

    /// <summary>
    /// Tools calling
    /// </summary>
    Tools = 5,

    /// <summary>
    /// Agent extracts data and output in a structured format (ex., JSON)
    /// </summary>
    DataExtraction = 6,

    /// <summary>
    /// Agent workflows
    /// </summary>
    Workflow = 7,

    /// <summary>
    /// Agency with at least two agents
    /// </summary>
    MultiAgent = 8,

    /// <summary>
    /// Lab.MultiModal, text, image (audio, video)
    /// </summary>
    MultiModal = 9,

    /// <summary>
    /// Lab.MultiModal, image processing
    /// </summary>
    Vision = 10,

    /// <summary>
    /// Sentiment analysis
    /// </summary>
    SentimentAnalyser = 11,

    /// <summary>
    /// Require Azure
    /// </summary>
    Azure = 12,

    /// <summary>
    /// CosmosDb persistence
    /// </summary>
    CosmosDb = 13,

    /// <summary>
    /// Tokens counting / output, stats
    /// </summary>
    Tokens = 14,

    /// <summary>
    /// Agent output streaming
    /// </summary>
    Streaming = 15,

    /// <summary>
    /// Agent uses the model context protocol
    /// </summary>
    Mcp = 16,

    /// <summary>
    /// Conversation compaction strategy
    /// </summary>
    Compaction = 17,

    /// <summary>
    /// The user is asked to input or answer questions from the agent
    /// </summary>
    UserInteraction = 18,

    /// <summary>
    /// A custom context provider is implemented
    /// </summary>
    AiContextProvider = 19,

    /// <summary>
    /// HITL, human in the loop for approvals, etc
    /// </summary>
    HumanInTheLoop = 20,

    /// <summary>
    /// Agent framework with Middleware
    /// </summary>
    Middleware = 21,

    /// <summary>
    /// Guardrail: the input / output from the agent is verified if it is appropriate, according to some policy
    /// </summary>
    Guardrail = 22,

    /// <summary>
    /// Optical Character Recognition (OCR)
    /// </summary>
    Ocr = 23,
}