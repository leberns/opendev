using McpFinanceExpenseApi.Models;
using McpFinanceExpenseApi.Models.Enums;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/expenses/{employeeId:int}", (int employeeId) =>
    {
        Console.WriteLine($"Get expenses of employee {employeeId}");

        List<GetExpenseResponseDto> expenses = [
            new (employeeId, "Lunch with client in Zurich", 63, "CHF", ExpenseStatusType.Approved)
        ];

        Console.WriteLine(expenses.Select(e => e.ToString()).Aggregate((a, b) => $"{a}, {b}"));

        return Results.Ok(expenses);
    })
    .WithName("GetExpenses");

app.MapPost("/api/expenses", (SubmitExpenseDto dto) =>
    {
        Console.WriteLine(dto);

        if(dto.Amount < 0)
        {
            return Results.BadRequest("Amount cannot be negative");
        }

        return Results.Ok();
    })
    .WithName("SubmitExpense");

app.Run();