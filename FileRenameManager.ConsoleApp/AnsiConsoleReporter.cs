using FileRenameManager.Core;
using Spectre.Console;

namespace FileRenameManager.ConsoleApp;

public sealed class AnsiConsoleReporter : IReporter
{
    private static readonly Lock _lock = new();
    public void Info(string message)
    {
        lock (_lock)
        {
            AnsiConsole.MarkupLine($"[bold deepskyblue1] INFO [/] {message} ");
        }
    }

    public void Warn(string message)
    {
        lock (_lock)
        {
            AnsiConsole.MarkupLine($"[bold yellow] WARN [/] {message} ");
        }
    }

    public void Error(string message, Exception? exception = null)
    {
        lock (_lock)
        {
            AnsiConsole.MarkupLine($"[bold red] ERROR [/] {message} ");
            if (exception != null)
            {
                AnsiConsole.MarkupLine(exception.ToString());
            }
        }
    }
}