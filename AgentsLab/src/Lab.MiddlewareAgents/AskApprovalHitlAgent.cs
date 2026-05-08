using Contracts;
using Contracts.ChatClientBuilders;
using Contracts.LabTags;

namespace Lab.MiddlewareAgents;

public class AskApprovalHitlAgent(
    IChatClientBuilder chatClientBuilder
    ) : IRunnableLab
{
    public List<TagType> GetTags()
    {
        throw new NotImplementedException();
    }

    public List<string> GetLabDescriptions()
    {
        throw new NotImplementedException();
    }

    public string GetUserInput()
    {
        throw new NotImplementedException();
    }

    public Task<string?> RunLabAsync(string userInput)
    {
        throw new NotImplementedException();
    }
}
