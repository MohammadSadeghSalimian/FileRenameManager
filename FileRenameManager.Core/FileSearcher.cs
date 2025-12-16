using System.Net.Http.Headers;

namespace FileRenameManager.Core;

public partial class FileSearcher(IReporter reporter, IImageFileProvider imageFileProvider, IVideoFileProvider videoFileProvider, IPhotoMetadataService imageMetadataService, IVideoMetadataService videoMetadataService) : IFileSearcher
{
    public async Task<IReadOnlyList<FileWithDate>> SearchInFolderAsync(
        DirectoryInfo destination, double hours,
        bool recursive = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(destination);
        if (!destination.Exists)
            throw new DirectoryNotFoundException(destination.FullName);

        reporter.Info($"Starting search operation in {destination.FullName} (recursive={recursive})");

        var images = imageFileProvider.GetImageFiles(destination, recursive);
        var videos = videoFileProvider.GetVideoFiles(destination, recursive);

        reporter.Info($"Found {images.Count} image file(s).");
        reporter.Info($"Found {videos.Count} video file(s).");

        var imagesWithDate = new List<FileWithDate>(images.Count);
        var videosWithDate = new List<FileWithDate>(videos.Count);

        // Offload metadata extraction to a background thread so caller (e.g. UI) is not blocked.
        await Task.Run(() =>
        {
            foreach (var file in images)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var photo = imageMetadataService.GetMediaWithDate(file, hours);
                imagesWithDate.Add(photo);
            }

            foreach (var file in videos)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var video = videoMetadataService.GetMediaWithDate(file, hours);
                videosWithDate.Add(video);
            }
        }, cancellationToken);

        return imagesWithDate.Concat(videosWithDate).ToList();
    }


    public async Task<IReadOnlyList<CycleFileWithDate>> SearchForFixedCameraImageAsync(DirectoryInfo source, bool recursive, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (!source.Exists)
            throw new DirectoryNotFoundException(source.FullName);

        reporter.Info($"Starting search operation in {source.FullName} (recursive={recursive})");

        var images = imageFileProvider.GetImageFiles(source, recursive);
        var videos = videoFileProvider.GetVideoFiles(source, recursive);
        var files = images.Concat(videos).ToList();
        reporter.Info($"Found {files.Count} file(s).");

        var cycleFiles = new List<CycleFileWithDate>(files.Count);

        await Task.Run(() =>
        {
            foreach (var file in files)
            {
                if (TryGetFixedCameraImageNames(file, out var cycleFile))
                {
                    cycleFiles.Add(cycleFile!);
                }
            }
        }, cancellationToken);

        return cycleFiles;
    }

    public async Task<IReadOnlyList<CycleFileWithDate>> SearchForFilesWithCycleNumberAsync(DirectoryInfo source, bool recursive, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (!source.Exists)
            throw new DirectoryNotFoundException(source.FullName);

        reporter.Info($"Starting search operation in {source.FullName} (recursive={recursive})");

        var images = imageFileProvider.GetImageFiles(source, recursive);
        var videos = videoFileProvider.GetVideoFiles(source, recursive);
        var files = images.Concat(videos).ToList();
        reporter.Info($"Found {files.Count} file(s).");
        var cycleFiles = new List<CycleFileWithDate>(files.Count);

        await Task.Run(() =>
        {
            foreach (var file in files)
            {
                if (TryToGetFilesWithCycleNumber(file, out var cycleFile))
                {
                    cycleFiles.Add(cycleFile!);
                }
            }
        }, cancellationToken);
        return cycleFiles;
    }


    private static bool TryGetFixedCameraImageNames(FileInfo file, out CycleFileWithDate? cycleFile)
    {
        // input: Left_12619 _2025-11-03-09-54-23_Cy-0.50
        //return Side_2023-12-01-10-47-51_Cy-4.50 (00375)
        var match = FixedCameraRegex().Match(file.Name);
        if (!match.Success)
        {
            cycleFile = null;
            return false;
        }

        var g = match.Groups;
        var prefix = g["prefix"].Value;
        var id = g["id"].Value;

        // Regex guarantees these are digits so Parse is safe and slightly faster than TryParse.
        var year = int.Parse(g["year"].Value);
        var month = int.Parse(g["month"].Value);
        var day = int.Parse(g["day"].Value);
        var hour = int.Parse(g["hour"].Value);
        var minute = int.Parse(g["minute"].Value);
        var seconds = int.Parse(g["seconds"].Value);

        // Cycle uses dot as decimal separator in the regex; parse with invariant culture.
        var cycle = double.Parse(g["cycle"].Value, System.Globalization.CultureInfo.InvariantCulture);

        var date = new DateTime(year, month, day, hour, minute, seconds);

        cycleFile = new CycleFileWithDate(file, date, cycle, prefix, id);
        return true;
    }

    private static bool TryToGetFilesWithCycleNumber(FileInfo file, out CycleFileWithDate? cycleFile)
    {
       
        var match = FilesWithCycleNumberRegex().Match(file.Name);
        if (!match.Success)
        {
            cycleFile = null;
            return false;
        }

        var g = match.Groups;
        var prefix = g["prefix"].Value;
        var id = g["id"].Value;
        var year = int.Parse(g["year"].Value);
        var month = int.Parse(g["month"].Value);
        var day = int.Parse(g["day"].Value);
        var hour = int.Parse(g["hour"].Value);
        var minute = int.Parse(g["minute"].Value);
        var seconds = int.Parse(g["second"].Value);

        // Cycle uses dot as decimal separator in the regex; parse with invariant culture.
        var cycle = double.Parse(g["cycle"].Value, System.Globalization.CultureInfo.InvariantCulture);

        var date = new DateTime(year, month, day, hour, minute, seconds);

        cycleFile = new CycleFileWithDate(file, date, cycle, prefix, id);
        return true;
    }


    [System.Text.RegularExpressions.GeneratedRegex(@"^(?<prefix>[A-z]+?)_*(?<id>\d+)\s*_(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})-(?<hour>\d{2})-(?<minute>\d{2})-(?<seconds>\d{2})_Cy-(?<cycle>\d+\.\d+).+(?<extension>\..+)$")]
    private static partial System.Text.RegularExpressions.Regex FixedCameraRegex();
    [System.Text.RegularExpressions.GeneratedRegex(@"^(?<prefix>.*?)(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})-(?<hour>\d{2})-(?<minute>\d{2})-(?<second>\d{2})[-_](?i:cy)[- ]?(?<cycle>\d+(?:\.\d+)?)(?<id_part>\s*\((?<id>\d+)\))?(?<extension>\.\w+)?$")]
    private static partial System.Text.RegularExpressions.Regex FilesWithCycleNumberRegex();
}