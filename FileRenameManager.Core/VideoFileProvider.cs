namespace FileRenameManager.Core;



public sealed class VideoFileProvider : IVideoFileProvider
{
    private readonly string[] _supportedExtensions = [".mp4", ".mov"];

    public IReadOnlyList<FileInfo> GetVideoFiles(DirectoryInfo rootDirectory, bool recursive)
    {
        ArgumentNullException.ThrowIfNull(rootDirectory);

        var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        return rootDirectory
            .EnumerateFiles("*.*", searchOption)
            .Where(f => _supportedExtensions.Contains(f.Extension, StringComparer.OrdinalIgnoreCase))
            .ToList();
    }
}