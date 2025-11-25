using FileRenameManager.Core;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace FileRenameManager.Infrastructures
{
    public sealed class PhotoMetadataService(IReporter reporter) : IPhotoMetadataService
    {
        private readonly IReporter _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));

        public FileWithDate GetMediaWithDate(FileInfo file,double hourOffset)
        {
            ArgumentNullException.ThrowIfNull(file);

            DateTime? dateTaken = null;

            try
            {
                dateTaken = TryReadDateTaken(file);
            }
            catch (Exception ex)
            {
                _reporter.Warn($"Failed to read EXIF metadata for: {file.FullName}");
                _reporter.Error("Metadata error", ex);
            }

            dateTaken ??= GuessFallbackDate(file);
            if (dateTaken != null && hourOffset != 0)
            {
                dateTaken = dateTaken.Value.AddHours(hourOffset);
            }

            return new FileWithDate(file, dateTaken);
        }

        private static DateTime? TryReadDateTaken(FileInfo file)
        {
            var directories = ImageMetadataReader.ReadMetadata(file.FullName);

            var subIfd = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            var date = subIfd?.GetDateTime(ExifDirectoryBase.TagDateTimeOriginal);
            if (date != null)
            {
                return date;
            }

            var exifIfd0 = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
            date = exifIfd0?.GetDateTime(ExifDirectoryBase.TagDateTime);

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
                _reporter.Error($"Failed to read file system dates for {file.FullName}", ex);
            }

            return null;
        }
    }
}
