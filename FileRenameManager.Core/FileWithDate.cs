namespace FileRenameManager.Core
{
    public sealed class FileWithDate(FileInfo file, DateTime? dateTaken)
    {
        public FileInfo File { get; } = file ?? throw new ArgumentNullException(nameof(file));
        public DateTime? DateTaken { get; } = dateTaken;

        
    }
}
