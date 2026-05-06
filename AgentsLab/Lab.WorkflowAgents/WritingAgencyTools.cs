using System.ComponentModel;

namespace WriterAgency;

public static class WritingAgencyTools
{
    [Description("Gets the author of the story.")]
    public static string GetAuthorTool() => "John Blue";

    [Description("Formats the story for display.")]
    public static string FormatStoryTool(string title, string author, string story) =>
        $"Title: {title}\nAuthor: {author}\n\n{story}";

}