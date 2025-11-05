namespace FileRenameManager.Core;

public interface IReporter
{
    void Info(string message);
    void Warn(string message);
    void Error(string message, Exception? exception = null);
}