namespace FileRenameManager.Core
{
    public sealed class PhotoWithDate(FileInfo file, DateTime? dateTaken)
    {
        public FileInfo File { get; } = file ?? throw new ArgumentNullException(nameof(file));
        public DateTime? DateTaken { get; } = dateTaken;
    }
}
