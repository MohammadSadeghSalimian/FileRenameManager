namespace FileRenameManager.Core;

public interface IFileSearcher
{
    Task<IReadOnlyList<FileWithDate>> SearchInFolderAsync(
        DirectoryInfo destination, double hours,
        bool recursive = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CycleFileWithDate>> SearchForFixedCameraImageAsync(DirectoryInfo source,bool recursive,CancellationToken cancellationToken=default);
}