using FileRenameManager.App;
using Microsoft.Extensions.Logging;

namespace FileRenameManager.ConsoleApp;

public class ConsoleMessage(ILogger<ConsoleMessage> logger) : IMessageUnit
{
    public Task ErrorAsync(string message)
    {
        logger.LogError(message);
        return Task.CompletedTask;
    }

    public Task WarningAsync(string message)
    {
        logger.LogWarning(message);
        return Task.CompletedTask;
    }

    public Task InfoAsync(string message)
    {
        logger.LogInformation(message);
        return Task.CompletedTask;
    }

    public Task ErrorAsync(Exception e)
    {
        logger.LogError(e, e.Message);
        return Task.CompletedTask;
    }
}