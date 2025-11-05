namespace FileRenameManager.Core;

public interface IVideoFileProvider
{
    IReadOnlyList<FileInfo> GetVideoFiles(DirectoryInfo rootDirectory, bool recursive);
}