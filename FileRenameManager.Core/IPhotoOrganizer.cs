namespace FileRenameManager.Core;

public interface IPhotoOrganizer
{
    void OrganizeByDate(DirectoryInfo rootDirectory, bool recursive = false, bool dryRun = false);
}