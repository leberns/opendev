# AgentsLab, Microsoft Agent Framework demos

A set of C# .NET Microsoft Agent Framework demos set to run mostly locally with Ollama.

I'm experimenting with agents and AI with C#, check a few demos and ideas here.

## Features

- Ollama, a local LLM server
- GitHub Copilot agents (optional for a few demos)
- Azure CosmosDB (optional for a few demos)
- Streaming, reasoning, thinking, tools, and more
- Agent memory and AI context providers
- Multi-modal demo
- Multi-agents demo with workflows and orchestration
- MCP (Model Context Protocol)
- Aspire to coordinate the MCP servers and REST APIs

## Labs

### Tokens & Streaming

| #   | Lab                                                                                                                               | Description                                                                                                                                                                    |
| --- | --------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 1   | [TokensStreamerAgent](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.TokensStreamer/TokensStreamerAgent.cs)       | Demonstrates response tokens, streaming and the counting of tokens usage. The instruction is to output the answer in capital case, just to demonstrate how instructions works. |
| 2   | [ReasoningThinkingAgent](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.TokensStreamer/ReasoningThinkingAgent.cs) | Demonstrates reasoning to resolve a complex problem. The thinking steps can be seen as they are streamed to the console.                                                       |

### Sentiment Analysis

| #   | Lab                                                                                                                                  | Description                                                                                                                                                                                                                     |
| --- | ------------------------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 3   | [SentimentAnalyserChat](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.SentimentAnalyser/SentimentAnalyserChat.cs)   | Demonstrates sentiment analysis from text using a LLM without an agent. The sentiment data is outputted in the chat answer. Temperature is set low so the most probable tokens are favoured, resulting in a less varied answer. |
| 4   | [SentimentAnalyserAgent](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.SentimentAnalyser/SentimentAnalyserAgent.cs) | Demonstrates the analysis and extraction of sentiment data from text using an agent. The sentiment data is outputted in an object. Temperature is set low for the same reason as above.                                         |

### Multi-Modal

| # | Lab                                                                                                                               | Description                                                                                                                                                                            |
|---| --------------------------------------------------------------------------------------------------------------------------------- |----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 5 | [ImageDataExtractionAgent](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.MultiModal/ImageDataExtractionAgent.cs) | Demonstrates an agent extracting visual information from an image — a form requesting an additional credit card (fields: card choice, name and address).                               |
| 6 | [PdfImageTextExtractionAgent](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.MultiModal/PdfImageTextExtractionAgent.cs) | Demonstrate an agent extracting text from images embedded in a PDF file. The PDF file can contain several pages, each page is rasterized to PNG and sent to the vision model for OCR.  |

### Multi-Turn Conversations

| #  | Lab                                                                                                                                              | Description                                                                                                                                                                                                                                                                       |
|----| ------------------------------------------------------------------------------------------------------------------------------------------------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 7  | [MeetingNoSession](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.MultiTurnConversation/MeetingNoSession.cs)                     | Demonstrates an agent without chat history. Because there is no history, the agent cannot answer questions based on previous user inputs.                                                                                                                                         |
| 8  | [MeetingHistoryInMemory](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.MultiTurnConversation/MeetingHistoryInMemory.cs)         | Demonstrates an agent with chat history stored in memory, enabling multi-turn conversations. Because the history is stored in memory it is not persisted between processes or requests.                                                                                           |
| 9  | [MeetingHistoryCosmosDb](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.MultiTurnConversation/MeetingHistoryCosmosDb.cs)         | Demonstrates an agent with chat history stored in Azure Cosmos DB. Requires the Azure Cosmos DB infrastructure (see prerequisites).                                                                                                                                               |
| 10 | [MeetingHistoryCosmosDbCont](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.MultiTurnConversation/MeetingHistoryCosmosDbCont.cs) | Demonstrates an agent continuing a conversation from a previous chat stored in Azure Cosmos DB. The conversation must have been started on the same day. Requires Azure Cosmos DB.                                                                                                |
| 11 | [TravelAdvisorAgent](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.MultiTurnConversation/TravelAdvisorAgent.cs)                 | Demonstrates a travel advisor agent with in memory chat history. Because there is history, the agent can answer questions based on previous messages (e.g. resolving pronouns like "there" to a previously mentioned city). The agent can also call a tool for weather forecasts. |
| 12 | [UserPreferencesAiContext](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.MultiTurnConversation/UserPreferencesAiContext.cs)     | Demonstrates an agent with chat history and a custom AI context provider that stores user preferences (location and language). The context provider injects these preferences into every agent invocation.                                                                        |
| 13 | [ConversationCompaction](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.MultiTurnConversation/ConversationCompaction.cs)         | Demonstrates conversation compaction — an experimental feature useful for reducing token usage in long conversations. Requires Azure Cosmos DB.                                                                                                                                   |

### Middleware & Guardrails

| #  | Lab                                                                                                                                     | Description                                                                                                                                                                                                                                                                                                                                                              |
|----| --------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 14 | [GuardrailMiddlewareAgent](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.MiddlewareAgents/GuardrailMiddlewareAgent.cs) | Demonstrates a guardrail middleware that filters out inappropriate content produced by a writer agent. A sensor agent detects and extracts inappropriate words into a findings record, separating detection from redaction. Note: the sensor agent can be replaced by deterministic logic for well-known patterns (e.g. emails, phone numbers, or other sensitive data). |

### Tools & Workflows

| #  | Lab                                                                                                                             | Description                                                                                                                                                                         |
|----| ------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 15 | [WriterAgent](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.WorkflowAgents/WriterAgent.cs)                     | Demonstrates an agent that creates a story and can use tools to format the output.                                                                                                  |
| 16 | [WorkflowWritingAgency](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.WorkflowAgents/WorkflowWritingAgency.cs) | Demonstrates a sequential agent workflow for story creation: a writer agent produces the story using formatting tools, and an editor agent reviews and makes changes.               |
| 17 | [AskForApproval](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.ToolsAgents/AskForApproval.cs)                  | Demonstrates human-in-the-loop control: a tool is wrapped by another tool that asks for user confirmation before it can be called, requiring the user to approve each agent action. |

### MCP (Model Context Protocol)

| #  | Lab                                                                                                                        | Description                                                                                                                                                                                                      |
|----| -------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 18 | [McpSubmitExpenseAgent](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.McpAgents/McpSubmitExpenseAgent.cs) | Demonstrates an agent calling a MCP service to create an expense. Architecture: `SubmitExpenseAgent → FinanceMCPClient → FinanceMCPServer → FinanceExpenseAPI`.                                                  |
| 19 | [McpServicesAgent](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.McpAgents/McpServicesAgent.cs)           | Demonstrates an agent that can perform several services by calling the appropriate MCP servers (Finance and IT Support). The input is intentionally incomplete, so the agent asks the user for more information. |
| 20 | [McpServicesAgency](https://github.com/leberns/opendev/blob/main/AgentsLab/src/Lab.McpAgents/McpServicesAgency.cs)         | Demonstrates a multi-agent architecture where a plan agent decomposes user requests and routes them to specialised Finance and IT Support agents, each backed by its own MCP server.                             |

---

Console menu, example:

```
Labs:                              Tags:
1 - TokensStreamerAgent            Streaming, Tokens
2 - ReasoningThinkingAgent         Streaming, Tokens, Thinking
3 - SentimentAnalyserChat          SentimentAnalyser, Chat
4 - SentimentAnalyserAgent         SentimentAnalyser, DataExtraction
5 - ImageDataExtractionAgent       MultiModal, Vision, DataExtraction
6 - MeetingNoSession
7 - MeetingHistoryInMemory         Conversation, ChatHistory
8 - MeetingHistoryCosmosDb         Conversation, ChatHistory, Azure, CosmosDb
9 - MeetingHistoryCosmosDbCont     Conversation, ChatHistory, Azure, CosmosDb
10 - TravelAdvisorAgent            Conversation, ChatHistory, Tools
11 - UserPreferencesAiContext      Conversation, ChatHistory, AiContextProvider, UserInteraction
12 - ConversationCompaction        Conversation, ChatHistory, Azure, CosmosDb, Compaction
13 - GuardrailMiddlewareAgent      DataExtraction, Middleware, Guardrail
14 - WriterAgent                   Tools
15 - WorkflowWritingAgency         Tools, MultiAgent, Workflow
16 - AskForApproval                Tools, UserInteraction, HumanInTheLoop
17 - McpSubmitExpenseAgent         Conversation, ChatHistory, Tools, Mcp
18 - McpServicesAgent              Conversation, ChatHistory, Tools, Mcp, UserInteraction
19 - McpServicesAgency             Conversation, ChatHistory, Tools, Mcp, MultiAgent, UserInteraction
Type a lab number or x to exit:

```

The Aspire resources, example:
![Aspire](docs/media/aspire-resources.png)

## Prerequisites

- .NET 10 SDK

- [Ollama](https://ollama.com/download)

- Pull the models so that Ollama has them locally:

  ```Sh
  ollama pull llama3.2
  ollama pull llama3.2-vision
  ollama pull qwen3.5:4b
  ollama pull qwen3-vl:4b
  ollama pull qwen3-vl:8b
  ```

### Prerequisites for GitHub Copilot (Optional)

If you like to run the Labs that depend on GitHub Copilot agents.

- set a GitHub Personal Access Token (PAT) with `models` scope: https://github.com/settings/tokens

  Save the PAT in an environment variable (`~/.zprofile`):

  ```Sh
  export GITHUB_TOKEN="YOUR-GITHUB-PAT"
  ```

### Prerequisites for Azure (Optional)

If you like to run the Labs that depend on Azure (like using the Azure CosmosBD).

- [Azure account](https://portal.azure.com/)

- [azd](https://learn.microsoft.com/en-us/azure/developer/azure-developer-cli/install-azd)

Provisioning the Azure Infrastructure:

```Sh
# preview what to provision on Azure (check subscription, location and resource group)
cd AgentsLab/src

azd provision --preview
```

```Sh
# provision the infrastructure on Azure
cd AgentsLab/src

azd provision
```

```Sh
# remove from Azure once you are done
cd AgentsLab/src

azd down
```

Update the Azure CosmosDb instance:

Create the database: `agentsdb` and the container: `history`.

Partition keys for the container: `/tenantId` -> `/userId` -> `/conversationId`

## Running the AgentsLab projects

```Sh
# clone the repo

cd AgentsLab/src

# run on each terminal or start both projects in an IDE

dotnet run --project AgentsLab.AppHost # start all needed web APIs and MCP servers (optional, not needed for all labs)

dotnet run --project AgentsLab.AppConsole # start the console app that host the agents and chats
```

## References

https://github.com/microsoft/agent-framework/

https://devblogs.microsoft.com/dotnet/introducing-microsoft-agent-framework-preview/

https://www.youtube.com/watch?v=wIUkjYJlEnU
