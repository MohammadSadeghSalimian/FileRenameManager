namespace FileRenameManager.Core
{
    public sealed class FileWithDate(FileInfo file, DateTime? dateTaken)
    {
        public FileInfo File { get; } = file ?? throw new ArgumentNullException(nameof(file));
        public DateTime? DateTaken { get; } = dateTaken;

        
    }


    public class CycleFileWithDate(FileInfo file, DateTime date, double cycleNumber, string prefix, string id)
    {
        public FileInfo File { get; } = file ?? throw new ArgumentNullException(nameof(file));
        public DateTime DateTaken { get; } = date;
        public double CycleNumber { get; } = cycleNumber;
        public string Prefix { get; } = prefix;
        public string Id { get; } = id;
        public string Extension => file.Extension;
    }

}
