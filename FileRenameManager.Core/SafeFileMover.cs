namespace FileRenameManager.Core;

public sealed class SafeFileMover(IReporter reporter) : IFileMover
{
    private readonly IReporter _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));

    public void MoveFileToFolder(FileInfo sourceFile, DirectoryInfo targetDirectory)
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

    public void MoveFile(FileInfo sourceFile, FileInfo destFile)
    {
        ArgumentNullException.ThrowIfNull(sourceFile);
        ArgumentNullException.ThrowIfNull(destFile);

        var targetDirectory = destFile.Directory;
        if (targetDirectory == null)
        {
            throw new InvalidOperationException("Destination file does not have a valid directory.");
        }

        if (!targetDirectory.Exists)
        {
            targetDirectory.Create();
            _reporter.Info($"Created directory: {targetDirectory.FullName}");
        }

        // Start with desired destination name
        var destPath = Path.Combine(targetDirectory.FullName, destFile.Name);

        // If exists, generate a unique name by appending " (n)" before the extension
        if (File.Exists(destPath))
        {
            var baseName = Path.GetFileNameWithoutExtension(destFile.Name);
            var ext = destFile.Extension;
            var counter = 1;
            string candidatePath;
            do
            {
                var candidateName = $"{baseName} ({counter}){ext}";
                candidatePath = Path.Combine(targetDirectory.FullName, candidateName);
                counter++;
            } while (File.Exists(candidatePath));

            destPath = candidatePath;
        }

        try
        {
            _reporter.Info($"Moving {sourceFile.FullName} -> {destPath}");
            sourceFile.MoveTo(destPath);
        }
        catch (Exception ex)
        {
            _reporter.Error($"Failed moving {sourceFile.FullName} -> {destPath}", ex);
            throw;
        }
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
    public void RenameFile(FileInfo sourceFile, string newFileName)
    {
        ArgumentNullException.ThrowIfNull(sourceFile);
        ArgumentNullException.ThrowIfNull(newFileName);
        var targetDirectory = sourceFile.Directory;
        if (targetDirectory == null)
        {
            throw new InvalidOperationException("Source file does not have a valid directory.");
        }
        var destPath = Path.Combine(targetDirectory.FullName, newFileName);
        if (File.Exists(destPath))
        {
            throw new IOException($"A file with the name {newFileName} already exists in the directory {targetDirectory.FullName}.");
        }
        _reporter.Info($"Renaming {sourceFile.FullName} -> {destPath}");
        sourceFile.MoveTo(destPath, false);
    }
}