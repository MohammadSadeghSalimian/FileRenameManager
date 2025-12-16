namespace FileRenameManager.App;

public interface IMessageUnit
{
    public Task ErrorAsync(string message);

    public Task WarningAsync(string message);
    public Task InfoAsync(string message);

    public Task ErrorAsync(Exception e);
}