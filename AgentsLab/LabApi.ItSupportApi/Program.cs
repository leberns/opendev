using McpItSupportApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/api/it-support", (CreateItSupportTicketDto dto) =>
    {
        Console.WriteLine(dto);

        return Results.Ok("The IT support ticket has been created.");
    })
    .WithName("CreateItSupportTicket");

app.Run();
