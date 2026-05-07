using System.ComponentModel;
using ModelContextProtocol.Server;

namespace ItSupportMCPServer.Resources;

[McpServerResourceType]
internal class ItSupportResources
{
    [McpServerResource(UriTemplate = "it-support://required-fields", Name = "IT Support Ticket Required Fields", MimeType = "text/plain")]
    [Description("The required fields to create an IT support ticket.")]
    public static string GetItSupportRequiredFields() =>
        """
        To create an IT support ticket you need: employee ID, and description.
        If any of these are missing, ask the user for them one by one before calling CreateItSupportTicket.
        Only call CreateItSupportTicket once you have all two values.
        """;
}
