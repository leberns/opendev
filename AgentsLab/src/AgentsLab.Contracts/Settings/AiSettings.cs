namespace Contracts.Settings;

/// <summary>
/// Settings for AI and connection to databases
/// </summary>
/// <param name="OllamaEndpoint">the Ollama URI</param>
/// <param name="AgentsDbAccountEndpoint">the Cosmos DB account endpoint URI,
/// ex.: https://cosmos-gwvqqxxl7josc.documents.azure.com:443/</param>
public record AiSettings(
    string OllamaEndpoint,
    string AgentsDbAccountEndpoint
);