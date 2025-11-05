using FileRenameManager.App;
using Spectre.Console;

namespace FileRenameManager.ConsoleApp;

public class CmMessageUnit: IMessageUnit
{
    public Task Error(string message)
    {
        AnsiConsole.WriteLine(message, new Style(Color.Red));
        return Task.CompletedTask;
    }

    public Task Warning(string message)
    {
        AnsiConsole.WriteLine(message, new Style(Color.Yellow));
        return Task.CompletedTask;
    }

    public Task Info(string message)
    {
        AnsiConsole.WriteLine(message, new Style(Color.Blue));
        return Task.CompletedTask;
    }

    public Task Error(Exception e)
    {
        AnsiConsole.WriteLine(e.Message, new Style(Color.Red));
        return Task.CompletedTask;
    }
}