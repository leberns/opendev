using System.ComponentModel;
using ModelContextProtocol.Server;

namespace McpServerExpense.Resources;

[McpServerResourceType]
internal class ExpenseResources
{
    [McpServerResource(UriTemplate = "expense://policy", Name = "Expense Policy", MimeType = "text/plain")]
    [Description("The expense policy rules that govern expense submissions.")]
    public static string GetPolicy() =>
        """
        Apply the following expense policy strictly:
        - Travel expenses are limited to 200 CHF.
        - Meal expenses are limited to 100 CHF.
        If an expense exceeds the limit, inform the user and do NOT create the expense.
        """;

    [McpServerResource(UriTemplate = "expense://required-fields", Name = "Expense Required Fields", MimeType = "text/plain")]
    [Description("The required fields to create an expense.")]
    public static string GetRequiredFields() =>
        """
        To register an expense you need: employee ID, description, amount, and currency.
        If any of these are missing, ask the user for them one by one before calling CreateExpense.
        Only call CreateExpense once you have all four values.
        """;
}
