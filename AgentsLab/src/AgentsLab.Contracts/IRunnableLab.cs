using Contracts.LabTags;

namespace Contracts;

/// <summary>
/// Interface to mark the classes as labs that can be executed.
/// </summary>
public interface IRunnableLab
{
    /// <summary>
    /// Get a set of tags describing the lab.
    /// </summary>
    List<TagType> GetTags();

    /// <summary>
    /// The lab descriptions.
    /// </summary>
    List<string> GetLabDescriptions();

    /// <summary>
    /// Return an initial user input to be prompted to the agent or chat.
    /// </summary>
    string GetUserInput();

    /// <summary>
    /// Execute the chat(s) or agent(s)
    /// </summary>
    /// <returns>Optional. The response to be displayed to the user after the agent execution.</returns>
    Task<string?>RunLabAsync(string userInput);
}