using Contracts;
using Microsoft.Extensions.Logging;

namespace AgentsConsole;

public class AgentsRunner(
    IEnumerable<IRunnableLab> runnableLabs,
    ILogger<AgentsRunner> logger
    )
{
    public async Task RunAsync()
    {
        logger.LogInformation("{DateTime} | Starting labs with chats and agents...", DateTime.Now);

        var labs = runnableLabs.ToList();

        bool continueRunning;
        do
        {
            var option = SelectOption(labs);

            continueRunning = await ExecuteOptionAsync(labs, option);

        } while (continueRunning);

        logger.LogInformation("{DateTime} | Done.", DateTime.Now);
    }

    private string? SelectOption(List<IRunnableLab> labs)
    {
        Console.WriteLine();
        Console.WriteLine("Labs:                              Tags:");

        var count = 1;
        foreach (var lab in labs)
        {
            var tags = string.Join(", ", lab.GetTags());

            var header = $"{count} - {lab.GetType().Name}";

            Console.WriteLine($"{header, -34} {tags}");

            count++;
        }

        Console.Write("Type a lab number or x to exit: ");
        var option = Console.ReadLine();

        return option;
    }

    private async Task<bool> ExecuteOptionAsync(List<IRunnableLab> labs, string? option)
    {
        if(string.IsNullOrEmpty(option))
        {
            return true;
        }

        var optionLower = option.ToLowerInvariant();

        if(optionLower.Equals("x", StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        int? selectedLabNumber = int.Parse(option);

        var selectedLab = labs.ElementAtOrDefault(selectedLabNumber.Value - 1);

        if (selectedLab is null)
        {
            logger.LogError("{DateTime} | No lab found for number {Count}", DateTime.Now, selectedLabNumber);
            return true;
        }

        var labName = selectedLab.GetType().Name;

        Console.WriteLine($"Lab {selectedLabNumber}: {labName}");

        try
        {
            var labDescriptions = selectedLab.GetLabDescriptions();

            Console.WriteLine();
            foreach (var description in labDescriptions)
            {
                Console.WriteLine(description);
            }

            var userInput = selectedLab.GetUserInput();

            if (!string.IsNullOrEmpty(userInput))
            {
                Console.WriteLine();
                Console.WriteLine("User Input:");
                Console.WriteLine(userInput);
            }

            PauseStarting();

            var result = await selectedLab.RunLabAsync(userInput);

            Console.WriteLine(string.IsNullOrEmpty(result) ? string.Empty : $"Result: {result}");
        }
        catch (OperationCanceledException exc)
        {
            Console.WriteLine($"The operation was cancelled whilst running the lab {labName}: {exc.Message}");
            return true;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error running the lab {labName}: {ex.Message}");
            Console.ResetColor();
        }

        PauseEnd();

        return true;
    }

    private void PauseStarting()
    {
        Console.WriteLine();
        Console.WriteLine("Press Enter to start the lab or x to exit...");

        var keyChar = WaitForKeyPressed();

        if (keyChar == 'x')
        {
            throw new OperationCanceledException();
        }
    }

    private void PauseEnd()
    {
        Console.WriteLine("The lab has finished, press Enter to continue...");

        WaitForKeyPressed();
    }

    private static char WaitForKeyPressed()
    {
        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            if (key.Key is ConsoleKey.Enter or ConsoleKey.X)
            {
                return key.KeyChar;
            }
        }
    }
}