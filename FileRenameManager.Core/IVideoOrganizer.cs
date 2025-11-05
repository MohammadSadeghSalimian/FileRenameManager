namespace FileRenameManager.Core;

public interface IVideoOrganizer
{
    Task OrganizeByDateAsync(
        DirectoryInfo rootDirectory,
        bool recursive = false,
        bool dryRun = false,
        CancellationToken cancellationToken = default);
}