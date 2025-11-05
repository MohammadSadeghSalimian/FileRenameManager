namespace FileRenameManager.Core;

public interface IFolderNameProvider
{
    string GetFolderName(DateTime dateTaken);
}