namespace FileRenameManager.Core;

public interface IImageFileProvider
{
    IReadOnlyList<FileInfo> GetImageFiles(DirectoryInfo rootDirectory, bool recursive);
}