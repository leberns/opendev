using System.ComponentModel;
using ModelContextProtocol.Server;

namespace FinanceMCPServer.Resources;

[McpServerResourceType]
internal class FinanceResources
{
    [McpServerResource(UriTemplate = "finance://expenses/policy", Name = "Expense Policy", MimeType = "text/plain")]
    [Description("The expense policy rules that govern expense submissions.")]
    public static string GetExpensePolicy() =>
        """
        Apply the following expense policy strictly:
        - Travel expenses are limited to 200 CHF.
        - Meal expenses are limited to 100 CHF.
        If an expense exceeds the limit, inform the user and do NOT submit the expense.
        """;

    [McpServerResource(UriTemplate = "finance://expenses/required-fields", Name = "Expense Required Fields", MimeType = "text/plain")]
    [Description("The required fields to get or submit an expense.")]
    public static string GetExpenseRequiredFields() =>
        """
        To get expenses you need: employee ID.
        If the employee ID is missing, ask the user for it before calling GetExpenses.

        To register an expense you need: employee ID, description, amount, and currency.
        The description should be one of: travel, meal, or other.
        If any of these are missing, ask the user for them one by one before calling SubmitExpense.
        Only call SubmitExpense once you have all four values.
        """;

    [McpServerResource(UriTemplate = "finance://tax-documents/required-fields", Name = "Tax Document Required Fields", MimeType = "text/plain")]
    [Description("The required fields to get tax documents.")]
    public static string GetTaxDocumentRequiredFields() =>
        """
        To get tax documents you need: employee ID, begin year, and end year.
        If any of these are missing, ask the user for them one by one before calling GetTaxDocuments.
        Only call GetTaxDocuments once you have all three values.
        """;
}
