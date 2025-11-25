namespace FileRenameManager.Core;

/// <summary>
/// Organizes image files into date-based folders using injected services.
/// The class coordinates discovery of image files, extraction of photo metadata,
/// folder name generation based on dates, moving files, and reporting progress or errors.
/// </summary>
/// <param name="fileProvider">Provider responsible for enumerating image files under a root directory.</param>
/// <param name="metadataService">Service used to extract metadata (including date taken) from image files.</param>
/// <param name="nameProvider">Provider that maps a <see cref="System.DateTime"/> to a folder name string.</param>
/// <param name="fileMover">Component responsible for moving files to a target directory.</param>
/// <param name="reporter">Reporter used to emit informational, warning and error messages.</param>
    public sealed class FileOrganizer(
    INameProvider nameProvider,
    IFileMover fileMover,
    IReporter reporter)
    : IFileOrganizer
{
   
  

    /// <summary>
    /// Provider that converts a <see cref="System.DateTime"/> into a folder name.
    /// </summary>
    private readonly INameProvider _nameProvider = nameProvider ?? throw new ArgumentNullException(nameof(nameProvider));

    /// <summary>
    /// Component responsible for performing file move operations.
    /// </summary>
    private readonly IFileMover _fileMover = fileMover ?? throw new ArgumentNullException(nameof(fileMover));

    /// <summary>
    /// Reporter used to log informational, warning and error messages.
    /// </summary>
    private readonly IReporter _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));

    
    public async Task MoveToDateFolderAsync(IReadOnlyList<FileWithDate> files, DirectoryInfo destination,
        bool dryRun = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(destination);
        if (!destination.Exists)
            throw new DirectoryNotFoundException(destination.FullName);


        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (file.DateTaken == null)
            {
                _reporter.Warn($"No usable date for {file.File.Name}, skipping.");
                continue;
            }

            var folderName = _nameProvider.GetFolderNameMmBased(file.DateTaken.Value);
            var targetDir = new DirectoryInfo(Path.Combine(destination.FullName, folderName));

            if (dryRun)
            {
                _reporter.Info($"[DRY-RUN] Would move {file.File.Name} → {targetDir.FullName}");
            }
            else
            {
                await Task.Run(() => _fileMover.MoveFileToFolder(file.File, targetDir), cancellationToken);
            }
        }

        _reporter.Info("Organization finished.");
    }


    public async Task RenameToDateNameBasedAsync(IReadOnlyList<FileWithDate> files, DirectoryInfo destination,
        bool dryRun = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(destination);
        if (!destination.Exists)
            throw new DirectoryNotFoundException(destination.FullName);

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (file.DateTaken == null)
            {
                _reporter.Warn($"No usable date for {file.File.Name}, skipping.");
                continue;
            }

            var newFileName = _nameProvider.GetFileNameBasedOnDate(file.DateTaken.Value, file.File.Extension);
            if (file.File.Directory == null)
            {
                _reporter.Warn($"No usable date for {file.File.Name}, skipping.");
                continue;
            }

           
            if (dryRun)
            {
                _reporter.Info($"[DRY-RUN] Would rename {file.File.Name} → {newFileName}");
            }
            else
            {
                await Task.Run(() =>
                {
                    try
                    {
                        _fileMover.RenameFile(file.File, newFileName);
                    }
                    catch (IOException e)
                    {
                        _reporter.Warn(e.Message);
                        _reporter.Warn("The file is skipped!");
                    }
                }, cancellationToken);
            }
        }
    }



    public async Task MoveCycleFilesAsync(IReadOnlyList<CycleFileWithDate> files, DirectoryInfo destination,
        bool dryRun = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(destination);
        if (!destination.Exists)
            throw new DirectoryNotFoundException(destination.FullName);

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var cx = Convert.ToInt32(Math.Ceiling(file.CycleNumber));
            var targetDir = new DirectoryInfo(Path.Combine(destination.FullName, file.Prefix, $"CY{cx}", file.CycleNumber.ToString("F2")));
            var newName = _nameProvider.GetNameCycleFile(file);
            var targetFile = new FileInfo(Path.Combine(targetDir.FullName, newName));

            if (dryRun)
            {
                _reporter.Info($"[DRY-RUN] Would move {file.File.Name} → {targetDir.FullName}");
            }
            else
            {
                await Task.Run(() =>
                {
                    Directory.CreateDirectory(targetDir.FullName);

                    _fileMover.MoveFile(file.File, targetFile);
                }, cancellationToken);
            }
        }

        _reporter.Info("Organization finished.");
    }
}