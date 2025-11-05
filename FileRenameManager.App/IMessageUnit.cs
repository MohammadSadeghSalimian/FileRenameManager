namespace FileRenameManager.App;

public interface IMessageUnit
{
    public Task Error(string message);

    public Task Warning(string message);
    public Task Info(string message);

    public Task Error(Exception e);
}