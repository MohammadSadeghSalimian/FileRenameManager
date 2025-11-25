using FileRenameManager.App;
using FileRenameManager.Core;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace FileRenameManager.ConsoleApp;

public class ConsoleMessage(ILogger<ConsoleMessage> logger) : IMessageUnit
{
    public Task Error(string message)
    {
        logger.LogError(message);
        return Task.CompletedTask;
    }

    public Task Warning(string message)
    {
        logger.LogWarning(message);
        return Task.CompletedTask;
    }

    public Task Info(string message)
    {
        logger.LogInformation(message);
        return Task.CompletedTask;
    }

    public Task Error(Exception e)
    {
        logger.LogError(e, e.Message);
        return Task.CompletedTask;
    }
}

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
