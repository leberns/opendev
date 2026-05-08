# Projects Creation

```Sh
cd ~/Dev/GitHub/Leberns/opendev/AgentsLab/src

dotnet new sln

dotnet new console -o AgentsConsole
dotnet sln add AgentsConsole

dotnet add package Microsoft.Extensions.Configuration --project AgentsConsole
dotnet add package Microsoft.Extensions.Configuration.Json --project AgentsConsole
dotnet add package Microsoft.Extensions.Configuration.Binder --project AgentsConsole
dotnet add package Microsoft.Extensions.DependencyInjection --project AgentsConsole # services.BuildServiceProvider
dotnet add package Microsoft.Extensions.Logging --project AgentsConsole
dotnet add package Microsoft.Extensions.Logging.Console --project AgentsConsole
# rename AgentsConsole to AgentsLab.AppConsole

dotnet new classlib -n Contracts
dotnet sln add Contracts

dotnet add package Microsoft.Extensions.AI.Abstractions --project Contracts

dotnet new classlib -n AgentExtensions
dotnet sln add AgentExtensions

dotnet add package Microsoft.Extensions.AI --project AgentExtensions
dotnet add package Microsoft.Agents.AI --project AgentExtensions

dotnet new classlib -n ChatClientBuilders
dotnet sln add ChatClientBuilders

dotnet add package Microsoft.Extensions.AI --project ChatClientBuilders
dotnet add package Microsoft.Extensions.AI.OpenAI --project ChatClientBuilders
dotnet add package Azure.Identity --project ChatClientBuilders
dotnet add package Azure.AI.OpenAI --project ChatClientBuilders

dotnet new classlib -n TokensStreamer
dotnet sln add TokensStreamer

dotnet add package Microsoft.Extensions.AI --project TokensStreamer
dotnet add package Microsoft.Agents.AI --project TokensStreamer

dotnet new classlib -n SentimentAnalyser
dotnet sln add SentimentAnalyser

dotnet add package Microsoft.Extensions.AI --project SentimentAnalyser
dotnet add package Microsoft.Agents.AI --project SentimentAnalyser

otnet new classlib -n WriterAgency
dotnet sln add WriterAgency

dotnet add package Microsoft.Extensions.AI --project WriterAgency
dotnet add package Microsoft.Agents.AI --project WriterAgency
dotnet add package Microsoft.Agents.AI.Workflows --project WriterAgency

dotnet new classlib -n MultiTurnConversation
dotnet sln add MultiTurnConversation

dotnet add package Microsoft.Extensions.AI --project MultiTurnConversation
dotnet add package Microsoft.Agents.AI --project MultiTurnConversation
dotnet add package Microsoft.Agents.AI.CosmosNoSql --project MultiTurnConversation

dotnet new classlib -n MultiModal
dotnet sln add MultiModal

dotnet add package Microsoft.Extensions.AI --project MultiModal
dotnet add package Microsoft.Agents.AI --project MultiModal

dotnet new classlib -n Lab.MiddlewareAgents
dotnet sln add Lab.MiddlewareAgents

dotnet add package Microsoft.Extensions.AI --project Lab.MiddlewareAgents
dotnet add package Microsoft.Agents.AI --project Lab.MiddlewareAgents

dotnet new classlib -n McpAgents
dotnet sln add McpAgents

dotnet add package Microsoft.Extensions.AI --project McpAgents
dotnet add package Microsoft.Agents.AI --project McpAgents

dotnet new mcpserver -n FinanceMCPServer
dotnet sln add FinanceMCPServer
# move the package references with version numbers from the .csproj file to Directory.Build.props

dotnet new webapi -n McpFinanceExpenseApi
dotnet sln add McpFinanceExpenseApi
# move the package references with version numbers from the .csproj file to Directory.Build.props

dotnet new webapi -n McpFinanceTaxApi
dotnet sln add McpFinanceTaxApi

dotnet new mcpserver -n ItSupportMCPServer
dotnet sln add ItSupportMCPServer

dotnet new webapi -n McpItSupportApi
dotnet sln add McpItSupportApi
```

```Sh
# adding Aspire to the project so that all projects start at once

dotnet new aspire-apphost -n AgentsLab.AppHost -o AgentsLab.AppHost 

dotnet new aspire-servicedefaults -n AgentsLab.ServiceDefaults -o AgentsLab.ServiceDefaults

dotnet sln add AgentsLab.AppHost
dotnet sln add AgentsLab.ServiceDefaults
```

```Sh
# initialization for the infrastructure configurations, used on the first time the project was implemented
cd ~/Dev/GitHub/Leberns/labs/ai/AgentsLab

azd init # provide as environment name: agents-lab, remove the AgentsConsole project (not required to provision it on Azure)

azd add # add new services or resources, like the CosmosDB
```
