namespace FileRenameManager.Core;

public sealed class DefaultFolderNameProvider : IFolderNameProvider
{
    public string GetFolderName(DateTime dateTaken)
    {
        // e.g. 2023-07-12 -> "23-07 July-12"
        return $"{dateTaken:yy-MM} {dateTaken:MMMM}-{dateTaken:dd}";
    }
}