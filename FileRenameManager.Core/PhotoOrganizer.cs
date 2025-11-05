namespace FileRenameManager.Core;

/// <summary>
/// Organizes image files into date-based folders using injected services.
/// The class coordinates discovery of image files, extraction of photo metadata,
/// folder name generation based on dates, moving files, and reporting progress or errors.
/// </summary>
/// <param name="fileProvider">Provider responsible for enumerating image files under a root directory.</param>
/// <param name="metadataService">Service used to extract metadata (including date taken) from image files.</param>
/// <param name="folderNameProvider">Provider that maps a <see cref="System.DateTime"/> to a folder name string.</param>
/// <param name="fileMover">Component responsible for moving files to a target directory.</param>
/// <param name="reporter">Reporter used to emit informational, warning and error messages.</param>
    public sealed class PhotoOrganizer(
    IImageFileProvider fileProvider,
    IPhotoMetadataService metadataService,
    IFolderNameProvider folderNameProvider,
    IFileMover fileMover,
    IReporter reporter)
    : IPhotoOrganizer
{
    /// <summary>
    /// Provider that supplies the image files to process.
    /// </summary>
    private readonly IImageFileProvider _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));

    /// <summary>
    /// Service used to obtain photo metadata (including the date taken).
    /// </summary>
    private readonly IPhotoMetadataService _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));

    /// <summary>
    /// Provider that converts a <see cref="System.DateTime"/> into a folder name.
    /// </summary>
    private readonly IFolderNameProvider _folderNameProvider = folderNameProvider ?? throw new ArgumentNullException(nameof(folderNameProvider));

    /// <summary>
    /// Component responsible for performing file move operations.
    /// </summary>
    private readonly IFileMover _fileMover = fileMover ?? throw new ArgumentNullException(nameof(fileMover));

    /// <summary>
    /// Reporter used to log informational, warning and error messages.
    /// </summary>
    private readonly IReporter _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));

    /// <summary>
    /// Organizes image files found under <paramref name="rootDirectory"/> into subfolders determined by the
    /// <see cref="IFolderNameProvider"/> based on each photo's date taken.
    /// </summary>
    /// <param name="rootDirectory">The root directory to search for image files. Must not be <c>null</c> and must exist.</param>
    /// <param name="recursive">
    /// If <c>true</c>, image files are discovered recursively in subdirectories; otherwise only the top-level directory is searched.
    /// Default is <c>false</c>.
    /// </param>
    /// <param name="dryRun">
    /// If <c>true</c>, no files are actually moved; instead the intended moves are reported via the <see cref="IReporter"/>.
    /// Default is <c>false</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="rootDirectory"/> is <c>null</c>.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when <paramref name="rootDirectory"/> does not exist on disk.</exception>
    /// <remarks>
    /// The method performs the following steps:
    /// 1. Validate inputs and log the start of organization.
    /// 2. Use <see cref="_fileProvider"/> to enumerate image files and report the count.
    /// 3. Query <see cref="_metadataService"/> for date information for each file.
    /// 4. For each photo:
    ///    - If no date is available, log a warning and skip the file.
    ///    - Otherwise, compute the target folder name using <see cref="_folderNameProvider"/>.
    ///    - If <paramref name="dryRun"/> is <c>true</c>, report the planned move; otherwise call <see cref="_fileMover"/> to move the file.
    /// 5. Log completion.
    /// </remarks>
    public void OrganizeByDate(DirectoryInfo rootDirectory, bool recursive = false, bool dryRun = false)
    {
        ArgumentNullException.ThrowIfNull(rootDirectory);
        if (!rootDirectory.Exists)
            throw new DirectoryNotFoundException(rootDirectory.FullName);

        _reporter.Info($"Starting organization in {rootDirectory.FullName} (recursive={recursive}, dryRun={dryRun})");

        var files = _fileProvider.GetImageFiles(rootDirectory, recursive);
        _reporter.Info($"Found {files.Count} image file(s).");

        var photos = GetPhotosWithDates(files);

        foreach (var photo in photos)
        {
            if (photo.DateTaken == null)
            {
                _reporter.Warn($"No usable date for file: {photo.File.FullName}. Skipping.");
                continue;
            }

            var date = photo.DateTaken.Value;
            var folderName = _folderNameProvider.GetFolderName(date);
            var targetDir = new DirectoryInfo(Path.Combine(rootDirectory.FullName, folderName));

            if (dryRun)
            {
                _reporter.Info($"[DRY-RUN] Would move {photo.File.Name} -> {targetDir.FullName}");
            }
            else
            {
                _fileMover.MoveFile(photo.File, targetDir);
            }
        }

        _reporter.Info("Organization finished.");
    }


    public async Task OrganizeByDateAsync(
        DirectoryInfo rootDirectory,
        bool recursive = false,
        bool dryRun = false,
        CancellationToken cancellationToken = default)
    {
        if (rootDirectory == null)
            throw new ArgumentNullException(nameof(rootDirectory));
        if (!rootDirectory.Exists)
            throw new DirectoryNotFoundException(rootDirectory.FullName);

        _reporter.Info($"Starting organization in {rootDirectory.FullName}");

        var files = _fileProvider.GetImageFiles(rootDirectory, recursive);
        var photos = new List<PhotoWithDate>(files.Count);

        // Extract metadata asynchronously
        await Task.Run(() =>
        {
            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var photo = _metadataService.GetPhotoWithDate(file);
                photos.Add(photo);
            }
        }, cancellationToken);

        foreach (var photo in photos)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (photo.DateTaken == null)
            {
                _reporter.Warn($"No usable date for {photo.File.Name}, skipping.");
                continue;
            }

            var folderName = _folderNameProvider.GetFolderName(photo.DateTaken.Value);
            var targetDir = new DirectoryInfo(Path.Combine(rootDirectory.FullName, folderName));

            if (dryRun)
            {
                _reporter.Info($"[DRY-RUN] Would move {photo.File.Name} → {targetDir.FullName}");
            }
            else
            {
                await Task.Run(() => _fileMover.MoveFile(photo.File, targetDir), cancellationToken);
            }
        }

        _reporter.Info("Organization finished.");
    }
    private IReadOnlyList<PhotoWithDate> GetPhotosWithDates(IReadOnlyList<FileInfo> files)
    {
        var result = new List<PhotoWithDate>(files.Count);

        foreach (var file in files)
        {
            var photo = _metadataService.GetPhotoWithDate(file);
            result.Add(photo);
        }

        return result;
    }
}