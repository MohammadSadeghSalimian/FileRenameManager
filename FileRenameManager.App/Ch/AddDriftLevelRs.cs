namespace FileRenameManager.App.Ch;

public class AddDriftLevelRs(bool isSuccess)
{
    public bool IsSuccess { get; } = isSuccess;

    public string Message { get; init; } = "";
}