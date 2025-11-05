namespace FileRenameManager.Core;

public sealed class SafeFileMover(IReporter reporter) : IFileMover
{
    private readonly IReporter _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));

    public void MoveFile(FileInfo sourceFile, DirectoryInfo targetDirectory)
    {
        ArgumentNullException.ThrowIfNull(sourceFile);
        ArgumentNullException.ThrowIfNull(targetDirectory);

        if (!targetDirectory.Exists)
        {
            targetDirectory.Create();
            _reporter.Info($"Created directory: {targetDirectory.FullName}");
        }

        var destPath = GetUniqueDestinationPath(sourceFile, targetDirectory);

        _reporter.Info($"Moving {sourceFile.FullName} -> {destPath}");
        sourceFile.MoveTo(destPath);
    }

    private static string GetUniqueDestinationPath(FileInfo file, DirectoryInfo targetDirectory)
    {
        var baseName = Path.GetFileNameWithoutExtension(file.Name);
        var ext = file.Extension;
        var destPath = Path.Combine(targetDirectory.FullName, file.Name);

        var counter = 1;
        while (File.Exists(destPath))
        {
            var candidateName = $"{baseName} ({counter}){ext}";
            destPath = Path.Combine(targetDirectory.FullName, candidateName);
            counter++;
        }

        return destPath;
    }
}