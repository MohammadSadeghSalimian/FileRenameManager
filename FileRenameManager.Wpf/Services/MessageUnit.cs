using System.Windows;
using FileRenameManager.App;
using Microsoft.Extensions.Logging;

namespace FileRenameManager.Wpf.Services;

public class MessageUnit(ILogger<MessageUnit> logger):IMessageUnit
{
    public Task Error(string message)
    {
       MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
       logger.LogError(message);
       return Task.CompletedTask;
    }

    public Task Warning(string message)
    {
        MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        logger.LogWarning(message);
        return Task.CompletedTask;
    }

    public Task Info(string message)
    {
        MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        logger.LogInformation(message);
        return Task.CompletedTask;
    }

    public Task Error(Exception e)
    {
        MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        logger.LogError(e, e.Message);
        return Task.CompletedTask;
    }
}