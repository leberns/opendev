var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/api/tax-documents/{employeeId}/{yearBegin}/{yearEnd}", (string employeeId, int yearBegin, int yearEnd) =>
    {
        Console.WriteLine($"Request tax documents for employee {employeeId} from {yearBegin} to {yearEnd}");

        return Results.Ok("The tax documents will be sent to your e-mail.");
    })
    .WithName("GetTaxDocuments");

app.Run();