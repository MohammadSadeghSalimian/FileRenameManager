namespace FileRenameManager.Core;

public sealed class ImageFileProvider : IImageFileProvider
{
    private readonly string[] _supportedExtensions =
    [
        ".jpg", ".jpeg", ".heic"
    ];

    public IReadOnlyList<FileInfo> GetImageFiles(DirectoryInfo rootDirectory, bool recursive)
    {
        ArgumentNullException.ThrowIfNull(rootDirectory);

        var searchOption = recursive
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;

        return rootDirectory
            .EnumerateFiles("*.*", searchOption)
            .Where(f => _supportedExtensions.Contains(f.Extension, StringComparer.OrdinalIgnoreCase))
            .ToList();
    }
}