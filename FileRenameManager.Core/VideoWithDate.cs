namespace FileRenameManager.Core;

public sealed class VideoWithDate(FileInfo file, DateTime? dateTaken)
{
    public FileInfo File { get; } = file ?? throw new ArgumentNullException(nameof(file));
    public DateTime? DateTaken { get; } = dateTaken;
}


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