using System.Diagnostics;

namespace FileRenameManager.Core;

public sealed class DebugReporter : IReporter
{
    public void Info(string message)
    {
        Debug.WriteLine($"[INFO] {message}");
    }

    public void Warn(string message)
    {
        Debug.WriteLine($"[WARN] {message}");
    }

    public void Error(string message, Exception? exception = null)
    {
        Debug.WriteLine($"[ERROR] {message}");
        if (exception != null)
        {
            Debug.WriteLine(exception.ToString());
        }
    }
}

