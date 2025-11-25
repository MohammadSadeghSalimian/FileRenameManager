using FileRenameManager.Core;
using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;

namespace FileRenameManager.Infrastructures;

public sealed class VideoMetadataService(IReporter reporter) : IVideoMetadataService
{
    private readonly IReporter _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));

    public FileWithDate GetMediaWithDate(FileInfo file, double hourOffset)
    {
        ArgumentNullException.ThrowIfNull(file);

        DateTime? dateTaken = null;
        try
        {
            dateTaken = TryReadVideoDate(file);
        }
        catch (Exception ex)
        {
            _reporter.Warn($"Failed to read video metadata: {file.FullName}");
            _reporter.Error("Metadata error", ex);
        }

        dateTaken ??= GuessFallbackDate(file);
        if (dateTaken != null)
        {
            dateTaken = dateTaken.Value.AddHours(hourOffset);
        }
        return new FileWithDate(file, dateTaken);
    }

    private static DateTime? TryReadVideoDate(FileInfo file)
    {
        var directories = ImageMetadataReader.ReadMetadata(file.FullName);

        // QuickTime-specific directory
        var quickTimeDir = directories.OfType<QuickTimeMovieHeaderDirectory>().FirstOrDefault();
        var date = quickTimeDir?.GetDateTime(QuickTimeMovieHeaderDirectory.TagCreated);
        if (date != null)
            return date;

        // Fallback: some MP4 files store creation in QuickTimeMetadataHeaderDirectory
        var quickMeta = directories.OfType<QuickTimeMetadataHeaderDirectory>().FirstOrDefault();
        date = quickMeta?.GetDateTime(QuickTimeMetadataHeaderDirectory.TagCreationDate);

        return date;
    }

    private DateTime? GuessFallbackDate(FileInfo file)
    {
        try
        {
            if (file.CreationTime != default)
                return file.CreationTime;
            if (file.LastWriteTime != default)
                return file.LastWriteTime;
        }
        catch (Exception ex)
        {
            _reporter.Error($"Failed to read file system date for {file.FullName}", ex);
        }

        return null;
    }
}