using System.ComponentModel;

namespace Lab.MiddlewareAgents;

public static class AskForApprovalTool
{
    [Description("Submit a support ticket.")]
    public static string SubmitSupportTicket(
        [Description("Description.")] string description)
    {
        Console.WriteLine($"Tool: submitting support ticket {description}");
        return "Success. The ticket was submitted.";
    }
}