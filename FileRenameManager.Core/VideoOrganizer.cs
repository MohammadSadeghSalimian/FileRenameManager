namespace FileRenameManager.Core;

public sealed class VideoOrganizer(
    IVideoFileProvider fileProvider,
    IVideoMetadataService metadataService,
    IFolderNameProvider folderNameProvider,
    IFileMover fileMover,
    IReporter reporter)
    : IVideoOrganizer
{
    private readonly IVideoFileProvider _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
    private readonly IVideoMetadataService _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
    private readonly IFolderNameProvider _folderNameProvider = folderNameProvider ?? throw new ArgumentNullException(nameof(folderNameProvider));
    private readonly IFileMover _fileMover = fileMover ?? throw new ArgumentNullException(nameof(fileMover));
    private readonly IReporter _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));

    public async Task OrganizeByDateAsync(
        DirectoryInfo rootDirectory,
        bool recursive = false,
        bool dryRun = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rootDirectory);
        if (!rootDirectory.Exists)
            throw new DirectoryNotFoundException(rootDirectory.FullName);

        _reporter.Info($"Organizing videos in {rootDirectory.FullName}");

        var files = _fileProvider.GetVideoFiles(rootDirectory, recursive);
        var videos = new List<VideoWithDate>(files.Count);

        await Task.Run(() =>
        {
            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();
                videos.Add(_metadataService.GetVideoWithDate(file));
            }
        }, cancellationToken);

        foreach (var video in videos)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (video.DateTaken == null)
            {
                _reporter.Warn($"No date found for {video.File.Name}, skipping.");
                continue;
            }

            var folderName = _folderNameProvider.GetFolderName(video.DateTaken.Value);
            var targetDir = new DirectoryInfo(Path.Combine(rootDirectory.FullName, folderName));

            if (dryRun)
            {
                _reporter.Info($"[DRY-RUN] Would move {video.File.Name} → {targetDir.FullName}");
            }
            else
            {
                await Task.Run(() => _fileMover.MoveFile(video.File, targetDir), cancellationToken);
            }
        }

        _reporter.Info("Video organization complete.");
    }
}