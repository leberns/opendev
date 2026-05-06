namespace McpFinanceExpenseApi.Models;

public record SubmitExpenseDto(
    int EmployeeId,
    string Description,
    decimal Amount,
    string Currency
    );