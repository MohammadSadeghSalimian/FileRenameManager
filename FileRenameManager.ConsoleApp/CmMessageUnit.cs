using FileRenameManager.App;
using Spectre.Console;

namespace FileRenameManager.ConsoleApp;

public class CmMessageUnit: IMessageUnit
{
    public Task ErrorAsync(string message)
    {
        AnsiConsole.WriteLine(message, new Style(Color.Red));
        return Task.CompletedTask;
    }

    public Task WarningAsync(string message)
    {
        AnsiConsole.WriteLine(message, new Style(Color.Yellow));
        return Task.CompletedTask;
    }

    public Task InfoAsync(string message)
    {
        AnsiConsole.WriteLine(message, new Style(Color.Blue));
        return Task.CompletedTask;
    }

    public Task ErrorAsync(Exception e)
    {
        AnsiConsole.WriteLine(e.Message, new Style(Color.Red));
        return Task.CompletedTask;
    }
}