using FileRenameManager.App;
using Microsoft.Extensions.Logging;

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