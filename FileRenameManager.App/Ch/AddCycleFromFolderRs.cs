namespace FileRenameManager.App.Ch;

public class AddCycleFromFolderRs(bool isSuccess)
{
    public bool IsSuccess { get; } = isSuccess;

    public string Message { get; init; } = "";
}